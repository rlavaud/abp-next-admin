<template>
  <div class="content">
    <Card>
      <CardGrid style="width: 25%">
        <CardMeta :title="L('Cloud')">
          <template #description>
            <DirectoryTree
              :tree-data="folderTree"
              :loadedKeys="loadedKeys"
              :expandedKeys="expandedKeys"
              @expand="fetchFolders"
              @select="handleSelectFolder"
            />
          </template>
        </CardMeta>
      </CardGrid>
      <CardGrid style="width: 75%">
        <component
          :is="componentsRef[switchComponent.name]"
          :select-group="switchComponent.group"
          :select-path="switchComponent.path"
          :delete-enabled="deleteEnabled"
          @append:folder="handleAppendFolder"
        />
      </CardGrid>
    </Card>
  </div>
</template>

<script lang="ts" setup>
  import { computed, ref, shallowRef } from 'vue';
  import { Card, Tree } from 'ant-design-vue';
  import { AntTreeNodeBaseEvent, TreeDataItem } from 'ant-design-vue/es/tree/Tree';
  import { useLocalization } from '/@/hooks/abp/useLocalization';
  import { usePermission } from '/@/hooks/web/usePermission';
  import { OssObject } from '/@/api/oss-management/objects/model';
  import FileList from './FileList.vue';
  import ShareList from './ShareList.vue';

  const componentsRef = shallowRef({
    FileList: FileList,
    ShareList: ShareList,
  });

  const CardGrid = Card.Grid;
  const CardMeta = Card.Meta;
  const DirectoryTree = Tree.DirectoryTree;

  interface IComponent {
    name: string;
    group: string;
    path: string;
    dataRef: any;
  }

  interface NodeInfo {
    path?: string;
    node: NodeInfo;
    parent?: NodeInfo;
  }

  const { hasPermission } = usePermission();
  const { L } = useLocalization(['AbpOssManagement', 'AbpUi']);
  const folderTreeRef = ref<{ [key: string]: TreeDataItem }>({
    private: {
      key: 'private',
      group: 'private',
      title: L('MyDocument'),
      path: '/',
      children: [],
    },
    public: {
      key: 'public',
      group: 'public',
      title: L('PublicDocument'),
      path: '/',
      children: [],
    },
    share: {
      key: 'share',
      group: 'share',
      title: L('MyShare'),
      path: '/',
      children: [],
    },
  });
  const folderTree = computed(() => {
    return Object.keys(folderTreeRef.value).map((key) => folderTreeRef.value[key]);
  });
  const switchComponent = ref<IComponent>({
    name: 'FileList',
    group: '',
    path: '/',
    dataRef: {},
  });
  const deleteEnabled = computed(() => {
    switch (switchComponent.value.group) {
      case 'private':
      case 'share':
        return true;
      case 'public':
        return hasPermission('AbpOssManagement.OssObject.Delete');
      default:
        return false;
    }
  });
  const loadedKeys = ref<string[]>([]);
  const expandedKeys = ref<string[]>([]);

  function fetchFolders(keys, e) {
    expandedKeys.value = keys;
    if (!e.expanded) {
      const keys = loadedKeys.value;
      const findIndex = keys.findLastIndex((key) => key === e.node.key);
      findIndex >= 0 && keys.splice(findIndex);
      loadedKeys.value = keys;
    }
  }

  function handleSelectFolder(_, e: AntTreeNodeBaseEvent) {
    const path = calculateFilePath(e.node as any);
    switch (e.node.group) {
      case 'private':
      case 'public':
        switchComponent.value = {
          name: 'FileList',
          group: e.node.group,
          path: path,
          dataRef: e.node.dataRef,
        };
        break;
      case 'share':
        switchComponent.value = {
          name: 'ShareList',
          group: e.node.group,
          path: path,
          dataRef: e.node.dataRef,
        };
        break;
    }
  }

  function handleAppendFolder(folders: OssObject[]) {
    switchComponent.value.dataRef.children = folders.map((obj) => {
      return {
        key: switchComponent.value.path + obj.name,
        group: switchComponent.value.group,
        title: obj.name,
        path: obj.name,
        children: [],
      };
    });
  }

  function calculateFilePath(e: NodeInfo) {
    let path = e.path ?? '';
    if (e.parent?.node?.path) {
      path = e.parent.node.path + path;
      path = calculateFilePath(e.parent) + path;
    }
    return path;
  }
</script>

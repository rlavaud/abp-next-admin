<script setup lang="ts">
import type { FormInstance } from 'ant-design-vue';

import type { IdentityRoleDto } from '../../types/roles';

import { defineEmits, defineOptions, ref, toValue } from 'vue';

import { useVbenModal } from '@vben/common-ui';
import { $t } from '@vben/locales';

import { Checkbox, Form, Input, message } from 'ant-design-vue';

import { createApi, getApi, updateApi } from '../../api/roles';

defineOptions({
  name: 'RoleModal',
});
const emits = defineEmits<{
  (event: 'change', data: IdentityRoleDto): void;
}>();

const FormItem = Form.Item;

const defaultModel = {
  isDefault: false,
  isPublic: true,
  isStatic: false,
} as IdentityRoleDto;

const form = ref<FormInstance>();
const formModel = ref<IdentityRoleDto>({ ...defaultModel });

const [Modal, modalApi] = useVbenModal({
  draggable: true,
  fullscreenButton: false,
  onCancel() {
    modalApi.close();
  },
  onConfirm: async () => {
    await form.value?.validate();
    const api = formModel.value.id
      ? updateApi(formModel.value.id, toValue(formModel))
      : createApi(toValue(formModel));
    modalApi.setState({ loading: true });
    api
      .then((res) => {
        message.success($t('AbpUi.Success'));
        emits('change', res);
        modalApi.close();
      })
      .finally(() => {
        modalApi.setState({ loading: false });
      });
  },
  onOpenChange: async (isOpen: boolean) => {
    if (isOpen) {
      const { values } = modalApi.getData<Record<string, any>>();
      if (values?.id) {
        modalApi.setState({ loading: true });
        return getApi(values.id)
          .then((dto) => {
            formModel.value = dto;
            modalApi.setState({
              title: $t('AbpIdentity.RoleSubject', [dto.name]),
            });
          })
          .finally(() => {
            modalApi.setState({ loading: false });
          });
      }
      formModel.value = { ...defaultModel };
      modalApi.setState({
        title: $t('NewRole'),
      });
    }
  },
  title: 'Roles',
});
</script>

<template>
  <Modal>
    <Form
      ref="form"
      :label-col="{ span: 6 }"
      :model="formModel"
      :wrapper-col="{ span: 18 }"
    >
      <FormItem :label="$t('AbpIdentity.DisplayName:IsDefault')">
        <Checkbox v-model:checked="formModel.isDefault">
          {{ $t('AbpIdentity.DisplayName:IsDefault') }}
        </Checkbox>
      </FormItem>
      <FormItem :label="$t('AbpIdentity.DisplayName:IsPublic')">
        <Checkbox v-model:checked="formModel.isPublic">
          {{ $t('AbpIdentity.DisplayName:IsPublic') }}
        </Checkbox>
      </FormItem>
      <FormItem
        :label="$t('AbpIdentity.DisplayName:RoleName')"
        name="name"
        required
      >
        <Input v-model:value="formModel.name" />
      </FormItem>
    </Form>
  </Modal>
</template>

<style scoped></style>

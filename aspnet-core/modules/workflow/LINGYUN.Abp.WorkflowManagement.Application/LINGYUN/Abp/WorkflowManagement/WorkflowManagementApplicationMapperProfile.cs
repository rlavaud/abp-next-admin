﻿using AutoMapper;
using LINGYUN.Abp.WorkflowManagement.Activitys;
using LINGYUN.Abp.WorkflowManagement.Workflows;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace LINGYUN.Abp.WorkflowManagement
{
    public class WorkflowManagementApplicationMapperProfile : Profile
    {
        public WorkflowManagementApplicationMapperProfile()
        {
            CreateMap<WorkflowInstance, WorkflowDto>()
                .ForMember(dto => dto.WorkflowId, map => map.MapFrom(src => src.Id.ToString()))
                .ForMember(dto => dto.DefinitionId, map => map.MapFrom(src => src.Id.ToString()))
                .ForMember(dto => dto.StartTime, map => map.MapFrom(src => src.CreateTime))
                .ForMember(dto => dto.EndTime, map => map.MapFrom(src => src.CompleteTime));
            CreateMap<PendingActivity, PendingActivityDto>();
        }
    }
}

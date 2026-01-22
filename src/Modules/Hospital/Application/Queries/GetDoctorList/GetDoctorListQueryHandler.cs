using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.HospitalAdmin.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Hospital.Application.Queries.GetDoctorList
{
    public class GetDoctorListQueryHandler : IRequestHandler<GetDoctorListQuery, Result<List<DoctorDto>>>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly ILogger<GetDoctorListQueryHandler> _logger;

        public GetDoctorListQueryHandler(
        IDoctorRepository doctorRepository,
        ILogger<GetDoctorListQueryHandler> logger)
        {
            _doctorRepository = doctorRepository;
            _logger = logger;
        }

        public Task<Result<List<DoctorDto>>> Handle(GetDoctorListQuery query, CancellationToken cancellationToken)
        {
            var doctorList = await _doctorRepository.GetDoctorList(query.HospNo, cancellationToken);

            List<DoctorDto> doctorDtoList = new List<DoctorDto>();

            foreach (var doctor in doctorList)
            {
                var doctorDto = new DoctorDto()
                {

                };

                doctorDtoList.Add(doctorDto);
            }

            return Result.Success<List<DoctorDto>>(doctorDtoList);
        }
    }
}

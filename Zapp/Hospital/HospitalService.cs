using AntPathMatching;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zapp.Schedule;

namespace Zapp.Hospital
{
    /// <summary>
    /// Represents a service that handles all the Zapp.Hospital queries.
    /// </summary>
    public class HospitalService : IHospitalService
    {
        private const string unknownPatientId = "Unknown";
        private const string deployReason = "Zapp is currently deploying.";

        private readonly IAntFactory antFactory;
        private readonly IScheduleService scheduleService;

        /// <summary>
        /// Initializes a new <see cref="HospitalService"/> with it's dependencies.
        /// </summary>
        /// <param name="antFactory">Factory used to create <see cref="IAnt"/> instances.</param>
        /// <param name="scheduleService">Service used to get info from <see cref="IFusionProcess"/> instances.</param>
        public HospitalService(
            IAntFactory antFactory,
            IScheduleService scheduleService)
        {
            this.antFactory = antFactory;
            this.scheduleService = scheduleService;
        }

        /// <summary>
        /// Gets all the statusses of the given query.
        /// </summary>
        /// <param name="fusionPattern">Pattern that is used to query <see cref="IFusionProcess"/> instances.</param>
        /// <param name="patientPattern">Pattern that is used to query <see cref="IPatient"/> instances.</param>
        /// <param name="token">Token to cancel the request.</param>
        /// <inheritDoc />
        public async Task<HospitalStatus> GetStatusAsync(
            string fusionPattern, 
            string patientPattern, 
            CancellationToken token)
        {
            EnsureArg.IsNotNullOrEmpty(fusionPattern, nameof(fusionPattern));
            EnsureArg.IsNotNullOrEmpty(patientPattern, nameof(patientPattern));

            var fusionMatch = antFactory
                .CreateNew(fusionPattern);

            var candidates = scheduleService.Processes
                .Where(_ => fusionMatch.IsMatch(_.FusionId))
                .OrderBy(_ => _.FusionId);

            var fusionStatusses = new List<FusionStatus>();

            foreach (var candidate in candidates)
            {
                var fusionStatus = await GetFusionStatusAsync(candidate, patientPattern, token);

                fusionStatusses.Add(fusionStatus);
            }

            var reason = string.Empty;
            var statusType = GetLowestType(fusionStatusses);
            
            if (statusType != PatientStatusType.Green)
            {
                if (scheduleService.IsDeploying())
                {
                    reason = deployReason;
                    statusType = PatientStatusType.Yellow;
                }
                else
                {
                    reason = $"One or more patients have the '{statusType.ToString()}' status.";
                }
            }

            return new HospitalStatus(fusionStatusses, statusType, reason);
        }

        private async Task<FusionStatus> GetFusionStatusAsync(
            IFusionProcess process, 
            string patientPattern, 
            CancellationToken token)
        {
            try
            {
                var patients = await process.NurseStatusAsync(patientPattern, token);

                return new FusionStatus(process.FusionId, patients);
            }
            catch (Exception ex)
            {
                return new FusionStatus(process.FusionId, new[] {
                    new PatientStatus(unknownPatientId, PatientStatusType.Red, ex.ToString())
                });
            }
        }

        private PatientStatusType GetLowestType(IEnumerable<FusionStatus> fusionStatusses)
        {
            return fusionStatusses
                .SelectMany(_ => _.Patients)
                .Select(_ => _.Type)
                .OrderBy(_ => _)
                .FirstOrDefault();
        }
    }
}

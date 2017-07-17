using AntPathMatching;
using EnsureThat;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using Zapp.Hospital;

namespace Zapp.Process.Hospital
{
    /// <summary>
    /// Represents a cluster of <see cref="IPatient"/> instances.
    /// </summary>
    public class PatientService : IPatientService
    {
        private const string defaultReason = "Something quite unexpected occured while requesting patient it's status.";

        private readonly IKernel kernel;
        private readonly IAntFactory antFactory;

        /// <summary>
        /// Initializes a new <see cref="PatientService"/> with it's dependencies.
        /// </summary>
        /// <param name="kernel">Current Ninject kernel that is active.</param>
        /// <param name="antFactory">Factory used to create <see cref="IAnt"/> instances.</param>
        public PatientService(
            IKernel kernel,
            IAntFactory antFactory)
        {
            this.kernel = kernel;
            this.antFactory = antFactory;
        }

        /// <summary>
        /// Gets all the statusses of tha <see cref="IPatient"/> instance that matches the pattern.
        /// </summary>
        /// <param name="patientPattern">Pattern to determine the instances of <see cref="IPatient"/>.</param>
        /// <inheritDoc />
        public IEnumerable<PatientStatus> GetStatusses(string patientPattern = "*")
        {
            EnsureArg.IsNotNullOrEmpty(patientPattern, nameof(patientPattern));

            var ant = antFactory
                .CreateNew(patientPattern);

            var patients = kernel.GetAll<IPatient>();

            var candidates = patients
                .Where(_ => ant.IsMatch(_.Id))
                .OrderBy(_ => _.Id);

            foreach (var candidate in candidates)
            {
                var status = new PatientStatus(candidate.Id, PatientStatusType.Red, defaultReason);

                try
                {
                    status = candidate.GetStatus() ?? status;
                }
                catch (Exception ex)
                {
                    status = new PatientStatus(candidate.Id, PatientStatusType.Red, ex.ToString());
                }

                yield return status;
            }
        }
    }
}

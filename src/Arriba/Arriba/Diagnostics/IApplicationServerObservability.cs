using System;
using System.Collections.Generic;

namespace Arriba.Diagnostics.SemanticLogging
{
    public interface IApplicationServerObservability
    {
        void RegisterChannel(string description);

        void RegisterApplication(string name);

        void RegisterContentReader(Type readerType, IEnumerable<string> contentTypes);

        void RegisterContentWriter(Type writerType, string contentType);

        void Starting(int applicationCount, int channelCount);

        void Stopping();
    }

    // todo: put in different dir/file
    public class ApplicationServerObservability : IApplicationServerObservability
    {
        private readonly IArribaTelemetry telemetry;

        public ApplicationServerObservability(IArribaTelemetry telemetry)
        {
            this.telemetry = telemetry;
        }

        public void RegisterChannel(string description)
        {
            this.telemetry.TrackInfo(
                nameof(RegisterChannel),
                new { description });
        }

        public void RegisterApplication(string name)
        {
            this.telemetry.TrackInfo(
                nameof(RegisterApplication),
                new { name });
        }

        public void RegisterContentReader(Type readerType, IEnumerable<string> contentTypes)
        {
            this.telemetry.TrackInfo(
                nameof(RegisterContentReader),
                new
                {
                    readerType,
                    contentTypes,
                });
        }

        public void RegisterContentWriter(Type writerType, string contentType)
        {
            this.telemetry.TrackInfo(
                nameof(RegisterContentWriter),
                new
                {
                    writerType,
                    contentType,
                });
        }

        public void Starting(int applicationCount, int channelCount)
        {
            this.telemetry.TrackInfo(
                nameof(Starting),
                new
                {
                    applicationCount,
                    channelCount,
                });
        }

        public void Stopping()
        {
            this.telemetry.TrackInfo(
                nameof(Stopping),
                new {});
        }
    }
}
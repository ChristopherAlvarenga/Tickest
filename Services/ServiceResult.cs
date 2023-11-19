using NuGet.Packaging;

namespace Tickest.Services
{
    public class ServiceResult
    {
        public ICollection<ServiceResultKeyError> Errors { get; set; }

        public bool Success => !Errors.Any();

        public object Data { get; set; }

        public void AddError(string key, string error) => Errors.Add(new(key, error));

        public ServiceResult(object data)
        {
            Data = data;
        }

        public ServiceResult()
        {
            Errors = new List<ServiceResultKeyError>();
        }

        public ServiceResult(ICollection<ServiceResultKeyError> errors)
        {
            Errors = errors ?? new List<ServiceResultKeyError>();
        }
    }

    public class ServiceResultKeyError
    {
        public ServiceResultKeyError(string key, string error)
        {
            Key = key;
            Error = error;
        }

        public string Key { get; set; }
        public string Error { get; set; }
    }
}

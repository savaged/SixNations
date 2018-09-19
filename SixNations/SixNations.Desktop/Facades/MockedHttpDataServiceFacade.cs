using log4net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using SixNations.Desktop.Constants;

namespace SixNations.Desktop.Facade
{
    public class MockedHttpDataServiceFacade : IHttpDataServiceFacade
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IList<Requirement> MockRequirementDb { get; }
        private IDictionary<string, string> MockLookupDb { get; }

        public MockedHttpDataServiceFacade()
        {
            MockRequirementDb = new List<Requirement>
            {
                new Requirement
                {
                    RequirementID = 75,
                    Story = "As a {user role}, I want to {the thing that is needed}, so that {the business value that allows prioritisation}.",
                    Estimation = 13,
                    Priority = 3,
                    Status = 2,
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 76,
                    Story = "As a standard user, I want to see savings for the previous day and week, so that {need the reason}.",
                    Estimation = 3,
                    Priority = 2,
                    Status = 1,
                    Release = ""
                },
                new Requirement
                {
                    RequirementID = 77,
                    Story = "As an Admin user, I want to add and edit roles, so that permissions can be managed more easily.",
                    Estimation = 5,
                    Priority = 1,
                    Status = 3,
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 78,
                    Story = "As an Admin user, I want to add and remove users, so that permissions can be managed more easily.",
                    Estimation = 8,
                    Priority = 1,
                    Status = 4,
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 79,
                    Story = "As an authorised user, I want to change my password, in case I have forgotten it, or so that I comply with the security policy to change it bi-monthly.",
                    Estimation = 1,
                    Priority = 3,
                    Status = 4,
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 80,
                    Story = "As a standard user, I want to add or edit email recipients, so that {need a reason here}.",
                    Estimation = 2,
                    Priority = 4,
                    Status = 4,
                    Release = ""
                }
            };

            MockLookupDb = new Dictionary<string, string>
            {
                { "RequirementEstimation", 
                    "{" +
                    "\"error\": \"\",  " +
                    "\"success\": true,  " +
                    "\"data\": [{" +
                    "      \"RequirementEstimationID\": 1," +
                    "      \"RequirementEstimationName\": \"XS\"," +
                    "    },    {" +
                    "      \"RequirementEstimationID\": 2," +
                    "      \"RequirementEstimationName\": \"Small\"," +
                    "    },    {" +
                    "      \"RequirementEstimationID\": 3," +
                    "      \"RequirementEstimationName\": \"Medium\"," +
                    "    },    {" +
                    "      \"RequirementEstimationID\": 5," +
                    "      \"RequirementEstimationName\": \"Large\"," +
                    "    },    {" +
                    "      \"RequirementEstimationID\": 8," +
                    "      \"RequirementEstimationName\": \"XL\"," +
                    "    },    {" +
                    "      \"RequirementEstimationID\": 13," +
                    "      \"RequirementEstimationName\": \"XXL\"," +
                    "    }]" +
                    "}"
                },
                { "RequirementPriority",
                    "{" +
                    "\"error\": \"\",  " +
                    "\"success\": true,  " +
                    "\"data\": [{" +
                    "      \"RequirementPriorityID\": 1," +
                    "      \"RequirementPriorityName\": \"Must\"," +
                    "    },    {" +
                    "      \"RequirementPriorityID\": 2," +
                    "      \"RequirementPriorityName\": \"Should\"," +
                    "    },    {" +
                    "      \"RequirementPriorityID\": 3," +
                    "      \"RequirementPriorityName\": \"Could\"," +
                    "    },    {" +
                    "      \"RequirementPriorityID\": 4," +
                    "      \"RequirementPriorityName\": \"Wont\"," +
                    "    }]" +
                    "}"
                },
                { "RequirementStatus",
                    "{" +
                    "\"error\": \"\",  " +
                    "\"success\": true,  " +
                    "\"data\": [{" +
                    "      \"RequirementStatusID\": 1," +
                    "      \"RequirementStatusName\": \"Prioritised\"," +
                    "    },    {" +
                    "      \"RequirementStatusID\": 2," +
                    "      \"RequirementStatusName\": \"WIP\"," +
                    "    },    {" +
                    "      \"RequirementStatusID\": 3," +
                    "      \"RequirementStatusName\": \"Test\"," +
                    "    },    {" +
                    "      \"RequirementStatusID\": 4," +
                    "      \"RequirementStatusName\": \"Done\"," +
                    "    }]" +
                    "}"
                }
            };
        }

        public async Task<IResponseRootObject> HttpRequestAsync(string uri, string token)
        {
            return await HttpRequestAsync(uri, token, API.Constants.HttpMethods.Get, null);
        }
        
        public async Task<IResponseRootObject> HttpRequestAsync(
            string uri, string token, API.Constants.HttpMethods httpMethod, IDictionary<string, object> data)
        {
            Log.Info($"Request initiated from {httpMethod}: {uri}");
            if (data?.Count > 0)
            {
                Log.DebugFormat("\twith request content: {0}", data);
            }
            var responseRootObject = GetResponse(httpMethod, uri, data);

            EmulateEditLocking(uri, httpMethod, ref responseRootObject);

            await Task.CompletedTask;

            Log.Info($"Request complete from {httpMethod}: {uri}");
            Log.Debug($"Response from {httpMethod}: {uri}-> {responseRootObject.ToString()}");
            return responseRootObject;
        }

        private IResponseRootObject GetResponse(
            API.Constants.HttpMethods httpMethod, string uri, IDictionary<string, object> data)
        {
            ResponseRootObject responseRootObject = null;
            switch (httpMethod)
            {
                case API.Constants.HttpMethods.Post:
                    responseRootObject = MockPost(uri, data);
                    break;
                case API.Constants.HttpMethods.Put:
                    responseRootObject = MockPut(uri, data);
                    break;
                case API.Constants.HttpMethods.Delete:
                    responseRootObject = MockDelete(uri);
                    break;
                default:
                    if (uri.EndsWith("create"))
                    {
                        responseRootObject = MockCreate(uri);
                    }
                    else if (uri.EndsWith("edit"))
                    {
                        responseRootObject = MockEdit(uri);
                    }
                    else
                    {
                        responseRootObject = MockGet(uri);
                    }
                    break;
            }
            return responseRootObject;
        }

        private ResponseRootObject MockPost(string uri, IDictionary<string, object> data)
        {
            var resource = GetResourceNameFromUri(uri);
            ResponseRootObject responseRootObject = null;
            switch (resource)
            {
                case "requirement":
                    var requirement = new Requirement();
                    var dto = Convert(data, uri);
                    requirement.Initialise(dto);
                    requirement.RequirementID = MockRequirementDb
                        .OrderByDescending(r => r.Id).FirstOrDefault().Id + 1;
                    MockRequirementDb.Add(requirement);
                    responseRootObject = Convert(requirement);
                    break;
                default:
                    throw new NotSupportedException();
            }
            return responseRootObject;
        }

        private ResponseRootObject MockPut(string uri, IDictionary<string, object> data)
        {
            var resource = GetResourceNameFromUri(uri);
            ResponseRootObject responseRootObject = null;
            switch (resource)
            {
                case "requirement":
                    var id = GetIdFromUri(uri);
                    var requirement = MockRequirementDb.FirstOrDefault(r => r.Id == id);
                    if (requirement != null)
                    {
                        MockRequirementDb.Remove(requirement);
                        requirement = new Requirement();
                        var dto = Convert(data, uri);
                        requirement.Initialise(dto);
                        MockRequirementDb.Add(requirement);
                    }
                    responseRootObject = Convert(requirement);
                    break;
                default:
                    throw new NotSupportedException();
            }
            return responseRootObject;
        }

        private ResponseRootObject MockDelete(string uri)
        {
            var resource = GetResourceNameFromUri(uri);
            ResponseRootObject responseRootObject = null;
            switch (resource)
            {
                case "requirement":
                    var id = GetIdFromUri(uri);
                    var requirement = MockRequirementDb.FirstOrDefault(r => r.Id == id);
                    if (requirement != null)
                    {
                        MockRequirementDb.Remove(requirement);
                    }
                    responseRootObject = JustSuccess(uri);
                    break;
                default:
                    throw new NotSupportedException();
            }
            return responseRootObject;
        }

        private ResponseRootObject MockCreate(string uri)
        {
            var resource = GetResourceNameFromUri(uri);
            ResponseRootObject responseRootObject = null;
            switch (resource)
            {
                case "requirement":
                    var @new = new Requirement();
                    responseRootObject = Convert(@new);
                    break;
                default:
                    throw new NotSupportedException();
            }
            return responseRootObject;
        }

        private ResponseRootObject MockEdit(string uri)
        {
            var resource = GetResourceNameFromUri(uri);
            ResponseRootObject responseRootObject = null;
            switch (resource)
            {
                case "requirement":
                    var id = GetIdFromUri(uri);
                    var requirement = MockRequirementDb.FirstOrDefault(r => r.Id == id);
                    responseRootObject = Convert(requirement);
                    break;
                default:
                    throw new NotSupportedException();
            }
            return responseRootObject;
        }

        private ResponseRootObject MockGet(string uri)
        {
            var resource = GetResourceNameFromUri(uri);
            ResponseRootObject responseRootObject = null;
            switch (resource)
            {
                case "requirement":
                    responseRootObject = Convert(MockRequirementDb);
                    break;
                case "requirementestimation":
                    responseRootObject = DeserializeResponseRootObject(
                        MockLookupDb["RequirementEstimation"], 200, $"Mocked/RequirementEstimation");
                    break;
                case "requirementpriority":
                    responseRootObject = DeserializeResponseRootObject(
                        MockLookupDb["RequirementPriority"], 200, $"Mocked/RequirementPriority");
                    break;
                case "requirementstatus":
                    responseRootObject = DeserializeResponseRootObject(
                        MockLookupDb["RequirementStatus"], 200, $"Mocked/RequirementStatus");
                    break;
            }
            return responseRootObject;
        }

        private int GetIdFromUri(string uri)
        {
            int value = 0;
            var match = Regex.Match(uri, @"\d+").Value;
            if (!string.IsNullOrEmpty(match))
            {
                value = int.Parse(match);
            }
            return value;
        }

        private string GetResourceNameFromUri(string uri)
        {
            var value = uri.Replace("Mocked/", string.Empty);
            var positionOfSlash = value.IndexOf("/");
            if (positionOfSlash > 0)
            {
                value = value.Substring(0, positionOfSlash);
            }
            return value;
        }

        private ResponseRootObject JustSuccess(string uri)
        {
            var json = "{\"error\":\"\",\"success\":true,\"data\":[]}";
            var value = DeserializeResponseRootObject(json, 200, uri);
            return value;
        }

        private DataTransferObject Convert(IDictionary<string, object> data, string uri)
        {
            var json = "{\"error\":\"\",\"success\":true,\"data\":[";
            json += JsonConvert.SerializeObject(data);
            json += "]}";
            var responseRootObject = DeserializeResponseRootObject(
                json, 200, uri);
            var value = (DataTransferObject)responseRootObject.Data.FirstOrDefault();
            return value;
        }

        private ResponseRootObject Convert(Requirement requirement)
        {
            var json = "{\"error\":\"\",\"success\":true,\"data\":[";
            json += JsonConvert.SerializeObject(requirement);
            json += "]}";
            var value = DeserializeResponseRootObject(
                json, 200, $"Mocked/requirement/{requirement.Id}");
            return value;
        }

        private ResponseRootObject Convert(IList<Requirement> requirements)
        {
            var json = "{\"error\":\"\",\"success\":true,\"data\":";
            json += JsonConvert.SerializeObject(requirements);
            json += "}";
            var value = DeserializeResponseRootObject(
                json, 200, "Mocked/requirement");
            return value;
        }

        private static ResponseRootObject DeserializeResponseRootObject(
            string rawResponseContent, int statusCode, string url)
        {
            var args = new object[]
            {
                rawResponseContent,
                statusCode,
                url
            };
            var methodInfo = typeof(HttpDataServiceFacade).GetMethod(
                nameof(DeserializeResponseRootObject), BindingFlags.NonPublic | BindingFlags.Static);

            var responseRootObject = (ResponseRootObject)methodInfo.Invoke(null, args);
            return responseRootObject;
        }

        private static void EmulateEditLocking(
            string url, API.Constants.HttpMethods httpMethod, ref IResponseRootObject responseRootObject)
        {
            if (responseRootObject == null)
            {
                throw new ArgumentNullException(nameof(responseRootObject));
            }
            if (httpMethod == API.Constants.HttpMethods.Get &&
                (url.EndsWith("edit") || url.Contains("create")))
            {
                foreach (var dto in responseRootObject.Data)
                {
                    var prop = typeof(DataTransferObject).GetProperty(
                        "Fields", BindingFlags.NonPublic | BindingFlags.Instance);
                    var fields = (IDictionary<string, object>)prop.GetValue(dto);

                    var key = "IsLockedForEditing";
                    if (fields.ContainsKey(key))
                    {
                        fields[key] = true;
                    }
                    else
                    {
                        fields.Add(key, true);
                    }
                }
            }
        }
    }
}
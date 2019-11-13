using System;
using System.Collections.Generic;

namespace savaged.mvvm.Data
{
    class UriActionBuilder
    {
        private readonly bool? _isIndexExplicitlyPlural;

        public UriActionBuilder(bool? isIndexExplicitlyPlural = null)
        {
            _isIndexExplicitlyPlural = isIndexExplicitlyPlural;
        }

        public string BuildSearch(Type resource)
        {
            return BuildSearch(resource.Name);
        }
        public string BuildSearch(string resource)
        {
            var uri = $"{resource.ToUriFormat()}search";
            return uri;
        }


        public string BuildIndex(
            Type resource, Type relation, int relationId)
        {
            var uri = resource?.Name?.ToUriFormat();

            if (IsValidRelation(relation, relationId))
            {
                uri += 
                    $"sby{relation.Name.ToUriFormat()}/{relationId}";
            }
            else if (_isIndexExplicitlyPlural == true)
            {
                uri += "s";
            }
            return uri;
        }

        public string BuildIndex(
            Type resource, 
            IDataModel relation = null,
            IDictionary<string, object> args = null)
        {
            return BuildIndex(resource.Name, relation, args);
        }
        public string BuildIndex(
            string resource, 
            IDataModel relation = null, 
            IDictionary<string, object> args = null)
        {
            var uri = resource.ToUriFormat();

            if (IsValidRelation(relation))
            {
                uri += $"sby{Convert(relation)}/{relation.Id}";
            }
            else if (_isIndexExplicitlyPlural == true)
            {
                uri += "s";
            }
            uri += GetArgsAsUriParams(args);
            return uri;
        }
        public string BuildIndex(
            Type resource,
            IEnumerable<IDataModel> related,
            IDictionary<string, object> args = null)
        {
            var uri = GetRelatedModelObjectIdsUri(related);
            uri += $"{resource.Name.ToUriFormat()}s";
            uri += GetArgsAsUriParams(args);
            return uri;
        }


        public string BuildCreate(
            Type resource, IDataModel relation = null)
        {
            return BuildCreate(resource.Name, relation);
        }
        public string BuildCreate(
            string resource, IDataModel relation = null)
        {
            var uri = resource.ToUriFormat();

            if (IsValidRelation(relation))
            {
                uri += $"by{Convert(relation)}/create/{relation.Id}";
            }
            else
            {
                uri += "/create";
            }
            return uri;
        }
        public string BuildCreate(
            Type resource,
            IEnumerable<IDataModel> related)
        {
            var uri = GetRelatedModelObjectIdsUri(related);
            uri += $"{resource.Name.ToUriFormat()}/create";
            return uri;
        }

        public string BuildStore(
            IDataModel resource, 
            IDataModel relation = null,
            IDictionary<string, object> args = null)
        {
            return BuildStore(Convert(resource), relation, args);
        }
        public string BuildStore(
            string resource, 
            IDataModel relation = null,
            IDictionary<string, object> args = null)
        {
            var uri = resource.ToUriFormat();

            if (IsValidRelation(relation))
            {
                uri += $"by{Convert(relation)}/{relation.Id}";
            }
            uri += GetArgsAsUriParams(args);
            return uri;
        }
        public string BuildStore(
            IDataModel resource,
            IList<IDataModel> related,
            IDictionary<string, object> args = null)
        {
            var uri = GetRelatedModelObjectIdsUri(related);
            uri += $"{resource.GetType().Name.ToUriFormat()}";
            uri += GetArgsAsUriParams(args);
            return uri;
        }


        public string BuildShow(
            IDataModel resource, IDictionary<string, object> args = null)
        {
            var uri = GetResourceAndIdUri(resource);
            uri += GetArgsAsUriParams(args);
            return uri;
        }

        public string BuildShow(
            string resource, 
            int id, 
            IDictionary<string, object> args = null)
        {
            var uri = $"{resource.ToUriFormat()}/{id}";
            uri += GetArgsAsUriParams(args);
            return uri;
        }

        public string BuildShow(
            Type resource, IDataModel relation = null,
            IDictionary<string, object> args = null)
        {
            var uri = resource.Name.ToUriFormat();

            if (IsValidRelation(relation))
            {
                uri += $"by{Convert(relation)}/{relation.Id}";
            }
            uri += GetArgsAsUriParams(args);
            return uri;
        }

        public string BuildShow(IEnumerable<IDataModel> related)
        {
            var uri = GetRelatedModelObjectIdsUri(related);
            if (uri.EndsWith("/"))
            {
                uri = uri.Remove(uri.Length - 1);
            }
            return uri;
        }


        public string BuildEdit(string resource, int id)
        {
            var uri = $"{resource.ToUriFormat()}/{id}/edit";
            return uri;
        }

        public string BuildEdit(IDataModel resource)
        {
            var uri = $"{GetResourceAndIdUri(resource)}/edit";
            return uri;
        }


        public string BuildUpdate(string resource, int id)
        {
            var uri = $"{resource.ToUriFormat()}/{id}";
            return uri;
        }

        public string BuildUpdate(IDataModel resource)
        {
            var uri = GetResourceAndIdUri(resource);
            return uri;
        }


        public string BuildDestroy(IDataModel resource)
        {
            var uri = GetResourceAndIdUri(resource);
            return uri;
        }


        public string BuildArchive(IDataModel resource)
        {
            var uri = $"{GetResourceAndIdUri(resource)}/archive";
            return uri;
        }


        public string BuildUnlock(IDataModel resource)
        {
            var uri = $"{GetResourceAndIdUri(resource)}/unlock";
            return uri;
        }


        private bool IsValidRelation(Type relation, int relationId)
        {
            var value = relation != null;
            value &= IsValidRelation(relationId);
            return value;
        }

        private bool IsValidRelation(int relationId)
        {
            bool value = relationId > 0;
            return value;
        }

        private bool IsValidRelation(IDataModel relation)
        {
            return relation != null && !relation.IsNew;
        }

        private string Convert(IDataModel resource)
        {
            return resource.GetType().Name.ToUriFormat();
        }

        private string GetResourceAndIdUri(IDataModel resource)
        {
            var uri = $"{Convert(resource)}/{resource.Id}";
            return uri.ToLower();
        }

        private string GetArgsAsUriParams(
            IDictionary<string, object> args)
        {
            var value = string.Empty;
            if (args != null && args.Count > 0)
            {
                value = args.ToUriParams();
            }
            return value;
        }


        private string GetRelatedModelObjectIdsUri(
            IEnumerable<IDataModel> related)
        {
            var uri = string.Empty;
            if (related != null)
            {
                foreach (var relation in related)
                {
                    if (IsValidRelation(relation))
                    {
                        uri += $"{relation.GetType().Name.ToUriFormat()}" +
                            $"/{relation.Id}/";
                    }
                }
            }
            return uri;
        }
        private string GetRelatedModelObjectIdsUri(
            IDictionary<Type, int> related)
        {
            var uri = string.Empty;
            foreach (var relation in related)
            {
                if (IsValidRelation(relation.Value))
                {
                    uri += $"{relation.Key.Name.ToUriFormat()}" +
                        $"/{relation.Value}/";
                }
            }
            return uri;
        }

    }
}

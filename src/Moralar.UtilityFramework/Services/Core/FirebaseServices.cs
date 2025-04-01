//using Firebase.Auth;
//using Firebase.Database;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;
//using UtilityFramework.Application.Core;
//using Moralar.UtilityFramework.Services.Core.Enum;
//using Moralar.UtilityFramework.Services.Core.Interface;
//using Moralar.UtilityFramework.Services.Core.Models;

//namespace Moralar.UtilityFramework.Services.Core
//{
//    public class FirebaseServices : IFirebaseServices
//    {
//        public static FirebaseClient ClientFirebase;

//        public static FirebaseConfigViewModel configFirebase;

//        public static FirebaseAuthProvider authProvider;

//        private string _json;

//        public static FirebaseAuthLink auth;

//        public FirebaseServices()
//        {
//            configFirebase = Utilities.GetConfigurationRoot().GetSection("FirebaseSettings").Get<FirebaseConfigViewModel>();
//            if (configFirebase == null)
//            {
//                throw new ArgumentNullException("FirebaseSettings", "Informe os dados de configuração do firebase em Config.json na prop \"FirebaseSettings\"");
//            }

//            if (string.IsNullOrEmpty(configFirebase.ApiKey))
//            {
//                throw new ArgumentNullException("ApiKey", "Informe a APIKEY do firebase");
//            }

//            authProvider = new FirebaseAuthProvider(new FirebaseConfig(configFirebase.ApiKey));
//            if (configFirebase.UseAuth)
//            {
//                if (auth == null)
//                {
//                    switch (configFirebase.TypeAuth)
//                    {
//                        case TypeAuthFirebase.Social:
//                            if (string.IsNullOrEmpty(configFirebase.SocialKey))
//                            {
//                                throw new ArgumentNullException("SocialKey", "Informe a SOCIALKEY do firebase");
//                            }

//                            auth = authProvider.SignInWithOAuthAsync(configFirebase.FirebaseAuthType, configFirebase.SocialKey).Result;
//                            break;
//                        case TypeAuthFirebase.EmailPassword:
//                            if (string.IsNullOrEmpty(configFirebase.Email))
//                            {
//                                throw new ArgumentNullException("Email", "Informe a Email de acesso ao firebase");
//                            }

//                            if (string.IsNullOrEmpty(configFirebase.Password))
//                            {
//                                throw new ArgumentNullException("Password", "Informe a Password de acesso ao firebase");
//                            }

//                            auth = authProvider.SignInWithEmailAndPasswordAsync(configFirebase.Email, configFirebase.Password).Result;
//                            break;
//                        default:
//                            auth = authProvider.SignInAnonymouslyAsync().Result;
//                            break;
//                    }

//                    ClientFirebase = new FirebaseClient(configFirebase.UrlDataBase, new FirebaseOptions
//                    {
//                        AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
//                    });
//                }
//                else
//                {
//                    if (auth.IsExpired() && authProvider != null)
//                    {
//                        auth = authProvider.RefreshAuthAsync(auth).Result;
//                    }

//                    if (ClientFirebase == null && configFirebase != null)
//                    {
//                        ClientFirebase = new FirebaseClient(configFirebase.UrlDataBase);
//                    }
//                }
//            }
//            else
//            {
//                ClientFirebase = new FirebaseClient(configFirebase.UrlDataBase);
//            }
//        }

//        public T Get<T>(string child)
//        {
//            return ClientFirebase.Child(child).OnceSingleAsync<T>().Result;
//        }

//        public void SavePost(string child, object data, JsonSerializerSettings configJson = null)
//        {
//            _json = ((configJson != null) ? JsonConvert.SerializeObject(data, configJson) : JsonConvert.SerializeObject(data));
//            ClientFirebase.Child(child).PostAsync(_json).Wait();
//        }

//        public void SavePut(string child, object data, JsonSerializerSettings configJson = null)
//        {
//            _json = ((configJson != null) ? JsonConvert.SerializeObject(data, configJson) : JsonConvert.SerializeObject(data));
//            ClientFirebase.Child(child).PutAsync(_json).Wait();
//        }

//        public void SavePatch(string child, object data, JsonSerializerSettings configJson = null)
//        {
//            _json = ((configJson != null) ? JsonConvert.SerializeObject(data, configJson) : JsonConvert.SerializeObject(data));
//            ClientFirebase.Child(child).PatchAsync(_json).Wait();
//        }

//        public void Delete(string child)
//        {
//            ClientFirebase.Child(child).DeleteAsync().Wait();
//        }

//        public IEnumerable<T> List<T>(string child)
//        {
//            return ClientFirebase.Child(child).OnceAsync<T>().Result.Select((FirebaseObject<T> x) => x.Object).ToList();
//        }

//        public async Task<T> GetAsync<T>(string child, bool configureAwait = false)
//        {
//            return await ClientFirebase.Child(child).OnceSingleAsync<T>().ConfigureAwait(configureAwait);
//        }

//        public async Task SavePostAsync(string child, object data, JsonSerializerSettings configJson = null, bool configureAwait = false)
//        {
//            _json = ((configJson != null) ? JsonConvert.SerializeObject(data, configJson) : JsonConvert.SerializeObject(data));
//            await ClientFirebase.Child(child).PostAsync(_json).ConfigureAwait(configureAwait);
//        }

//        public async Task SavePutAsync(string child, object data, JsonSerializerSettings configJson = null, bool configureAwait = false)
//        {
//            _json = ((configJson != null) ? JsonConvert.SerializeObject(data, configJson) : JsonConvert.SerializeObject(data));
//            await ClientFirebase.Child(child).PutAsync(_json).ConfigureAwait(configureAwait);
//        }

//        public async Task SavePatchAsync(string child, object data, JsonSerializerSettings configJson = null, bool configureAwait = false)
//        {
//            _json = ((configJson != null) ? JsonConvert.SerializeObject(data, configJson) : JsonConvert.SerializeObject(data));
//            await ClientFirebase.Child(child).PatchAsync(_json).ConfigureAwait(configureAwait);
//        }

//        public async Task DeleteAsync(string child, bool configureAwait = false)
//        {
//            await ClientFirebase.Child(child).DeleteAsync().ConfigureAwait(configureAwait);
//        }

//        public async Task<IEnumerable<T>> ListAsync<T>(string child, bool configureAwait = false)
//        {
//            return (await ClientFirebase.Child(child).OnceAsync<T>().ConfigureAwait(configureAwait)).Select((FirebaseObject<T> x) => x.Object);
//        }
//    }
//}

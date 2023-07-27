using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SlotMaker.Json;

namespace SlotMaker.TestSuite
{
    public class AsanaHttp : MonoSingleton<AsanaHttp>
    {
        private readonly string baseUrl = "https://app.asana.com/api/1.0";

        private class CreateTaskRequest
        {
            public CreateTaskRequestData data;
        }

        private class CreateTaskRequestData
        {
            public string name;
            public string notes;
            public long workspace;
            public List<long> projects;
            public Dictionary<string, string> custom_fields;
        }

        private class CreateTaskResponse
        {
            public CreateTaskResponseData data = null;
        }

        private class CreateTaskResponseData
        {
            public long id = 0;
        }

        public void CreateTask(string accessToken, Dictionary<string, object> report)
        {
            StartCoroutine(Tasks(accessToken, report));
        }

        // private void AttachText(string accessToken, long taskId, string fileName, string fileData)
        // {
        //     byte[] bytes = Encoding.UTF8.GetBytes(fileData);
        //     StartCoroutine(Attachments(accessToken, taskId, bytes, fileName, "text/plain"));
        // }

        // private void AttachImage(string accessToken, long taskId, string fileName)
        // {
        //     byte[] bytes = File.ReadAllBytes(Application.temporaryCachePath + "/" + fileName);
        //     StartCoroutine(Attachments(accessToken, taskId, bytes, fileName, "image/png"));
        // }

        public IEnumerator Tasks(string accessToken, Dictionary<string, object> report)
        {
            var createTaskRequest = new CreateTaskRequest();
            var createTaskData = new CreateTaskRequestData();
            createTaskData.name = (string)report["name"];
            createTaskData.notes = (string)report["notes"];
            createTaskData.workspace = (long)report["workspace"];
            createTaskData.projects = (List<long>)report["projects"];
            createTaskData.custom_fields = (Dictionary<string, string>)report["custom_fields"];
            createTaskRequest.data = createTaskData;
            string json = SlotSimpleJson.SerializeObject(createTaskRequest);

            WWWForm form = new WWWForm();
            form.AddField("", "");
            UnityWebRequest request = UnityWebRequest.Post(baseUrl + "/tasks", form);
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", accessToken));
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            yield return request.SendWebRequest();

            Debug.Log(request.downloadHandler.text);
            var createTaskResponse = SlotSimpleJson.DeserializeObject<CreateTaskResponse>(request.downloadHandler.text);
            if(createTaskResponse != null && createTaskResponse.data != null)
            {
                long taskId = createTaskResponse.data.id;

                object details;
                if (report.TryGetValue("details", out details))
                {
                    bytes = Encoding.UTF8.GetBytes(SlotSimpleJson.SerializeObject(details));
                    yield return StartCoroutine(Attachments(accessToken, taskId, bytes, "details.json", "text/plain"));
                }

                object screenshot;
                if (report.TryGetValue("screenshot", out screenshot))
                {
                    bytes = File.ReadAllBytes(Application.temporaryCachePath + "/" + (string)screenshot);
                    yield return StartCoroutine(Attachments(accessToken, taskId, bytes, (string)screenshot, "image/png"));
                }
            }
            
        }

        private IEnumerator Attachments(string accessToken, long taskId, byte[] bytes, string fileName, string contentType)
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("file", bytes, fileName, contentType));
            UnityWebRequest request = UnityWebRequest.Post(string.Format("{0}/tasks/{1}/attachments", baseUrl, taskId), formData, UnityWebRequest.GenerateBoundary());
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", accessToken));
            yield return request.SendWebRequest();
        }
    }
}

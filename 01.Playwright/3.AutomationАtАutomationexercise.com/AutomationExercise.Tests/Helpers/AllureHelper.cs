using Allure.Net.Commons;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AutomationExercise.Tests.Helpers
{
    public static class AllureHelper
    {
        public static async Task StepAsync(string stepName, Func<Task> action)
        {
            await AllureApi.Step(stepName, action);
        }

        public static async Task<T> StepAsync<T>(string stepName, Func<Task<T>> action)
        {
            return await AllureApi.Step(stepName, action);
        }

        public static void AddTextAttachment(string name, string content)
        {
            AllureApi.AddAttachment(name, "text/plain", content);
        }

        public static void AddJsonAttachment(string name, string json)
        {
            AllureApi.AddAttachment(name, "application/json", json);
        }

        public static void AddScreenshotAttachment(string name, byte[] screenshotBytes)
        {
            AllureApi.AddAttachment(name, "image/png", screenshotBytes);
        }
        
        public static void AddScreenshotAttachment(string name, string path)
        {
            AllureApi.AddAttachment(name, "image/png", path);
        }
    }
}
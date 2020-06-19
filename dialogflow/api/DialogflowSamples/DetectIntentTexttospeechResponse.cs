// Copyright(c) 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.IO;
using CommandLine;
using Google.Cloud.Dialogflow.V2;

namespace GoogleCloudSamples
{
    // Samples demonstrating how to detect Intents and Texttospeech Response
    public class DetectIntentTexttospeechResponse
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DetectIntentTexttospeechResponseFromTextsOptions opts) =>
                     DetectIntentTexttospeechResponseFromTexts(opts.ProjectId, opts.SessionId, opts.Texts, opts.LanguageCode));
        }

        [Verb("detect-intent:texttospeech", HelpText = "Detect Intent texttospeech response")]
        public class DetectIntentTexttospeechResponseFromTextsOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "texts", HelpText = "Comma separated text input", Required = true)]
            public string TextsInput { get; set; }

            public string[] Texts => TextsInput.Split(',');

            [Value(1, MetaName = "languageCode", HelpText = "Language code, eg. en-US", Default = "en-US")]
            public string LanguageCode { get; set; }
        }

        // [START dialogflow_detect_intent_texttospeech_response]
        public static int DetectIntentTexttospeechResponseFromTexts(string projectId,
                                                string sessionId,
                                                string[] texts,
                                                string languageCode = "en-US")
        {
            var client = SessionsClient.Create();
            var outputAudioConfig = new OutputAudioConfig
            {
                AudioEncoding = OutputAudioEncoding.Linear16,
                SynthesizeSpeechConfig = new SynthesizeSpeechConfig
                {
                    SpeakingRate = 1,
                    Pitch = 1,
                    VolumeGainDb = 1
                },
                SampleRateHertz = 16000,
            };

            foreach (var text in texts)
            {
                var response = client.DetectIntent(new DetectIntentRequest
                    {
                        SessionAsSessionName = SessionName.FromProjectSession(projectId, sessionId),
                        OutputAudioConfig = outputAudioConfig,
                        QueryInput = new QueryInput()
                        {
                            Text = new TextInput()
                            {
                                Text = text,
                                LanguageCode = languageCode
                            }
                        }
                    }
                );

                var queryResult = response.QueryResult;

                Console.WriteLine($"Query text: {queryResult.QueryText}");
                if (queryResult.Intent != null)
                {
                    Console.WriteLine($"Intent detected: {queryResult.Intent.DisplayName}");
                }
                Console.WriteLine($"Intent confidence: {queryResult.IntentDetectionConfidence}");
                Console.WriteLine($"Fulfillment text: {queryResult.FulfillmentText}");
                Console.WriteLine();

                if (response.OutputAudio.Length > 0)
                {
                    using (var output = File.Create("output.wav"))
                    {
                        response.OutputAudio.WriteTo(output);
                    }
                    Console.WriteLine("Audio content written to file \"output.wav\"");
                }
            }

            return 0;
        }
        // [END dialogflow_detect_intent_texttospeech_response]
    }
}

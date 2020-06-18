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
using System.Linq;
using System.Collections.Generic;
using CommandLine;
using Google.Cloud.Dialogflow.V2;

namespace GoogleCloudSamples
{
    // Samples demonstrating how to detect Intents using texts
    public class DetectIntentAudioFile
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DetectIntentFromTextsOptions opts) =>
                     DetectIntentFromTexts(opts.ProjectId, opts.SessionId, opts.Texts, opts.LanguageCode));
        }

        [Verb("detect-intent:audio", HelpText = "Detect intent from audio")]
        public class DetectIntentAudioFileOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "file", HelpText = "Path to the audio file", Required = true)]
            public string FilePath { get; set; }
        }

        // [START dialogflow_detect_intent_audio_file]
        public static int DetectIntentFromAudioFile(string projectId,
                                                string sessionId,
                                                string filePath,
                                                string languageCode = "en-US")
        {
            var client = SessionsClient.Create();

            var buffer = new byte[32 * 1024];
            int bytesRead;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                bytesRead = fileStream.Read(buffer);
            }

            var response = sessionsClient.DetectIntent(new DetectIntentRequest
            {
                SessionAsSessionName = SessionName.FromProjectSession(projectId, sessionId),
                OutputAudioConfig = outputAudioConfig,
                InputAudio = Google.Protobuf.ByteString.CopyFrom(buffer),
                QueryInput = new QueryInput()
                {
                    AudioConfig = new InputAudioConfig()
                    {
                        AudioEncoding = AudioEncoding.Linear16,
                        LanguageCode = "en-US",
                        SampleRateHertz = 16000
                    }
                }
            });

            var queryResult = response.QueryResult;

            Console.WriteLine($"Query text: {queryResult.QueryText}");
            if (queryResult.Intent != null)
            {
                Console.WriteLine($"Intent detected: {queryResult.Intent.DisplayName}");
            }
            Console.WriteLine($"Intent confidence: {queryResult.IntentDetectionConfidence}");
            Console.WriteLine($"Fulfillment text: {queryResult.FulfillmentText}");
            Console.WriteLine();

            return 0;
        }
        // [END dialogflow_detect_intent_audio_file]
    }
}

﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.ServiceBus.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class OnMessageTopicSubscriptionTests : SenderReceiverClientTestBase
    {
        public static IEnumerable<object> TestPermutations => new object[]
        {
            new object[] { TestUtility.NamespaceConnectionString, TestConstants.NonPartitionedTopicName, 5 },
            new object[] { TestUtility.NamespaceConnectionString, TestConstants.PartitionedTopicName, 5 },
            new object[] { TestUtility.WebSocketsNamespaceConnectionString, TestConstants.NonPartitionedTopicName, 5 },
            new object[] { TestUtility.WebSocketsNamespaceConnectionString, TestConstants.PartitionedTopicName, 5 },
        };

        string SubscriptionName => TestConstants.SubscriptionName;

        [Theory]
        [MemberData(nameof(TestPermutations))]
        [DisplayTestMethodName]
        async Task OnMessagePeekLockWithAutoCompleteTrue(string connectionString, string topicName, int maxConcurrentCalls)
        {
            await this.OnMessageTestAsync(connectionString, topicName, maxConcurrentCalls, ReceiveMode.PeekLock, true);
        }

        [Theory]
        [MemberData(nameof(TestPermutations))]
        [DisplayTestMethodName]
        async Task OnMessageReceiveDelete(string connectionString, string topicName, int maxConcurrentCalls)
        {
            await this.OnMessageTestAsync(connectionString, topicName, maxConcurrentCalls, ReceiveMode.ReceiveAndDelete, false);
        }

        async Task OnMessageTestAsync(string connectionString, string topicName, int maxConcurrentCalls, ReceiveMode mode, bool autoComplete)
        {
            const int messageCount = 10;

            var topicClient = new TopicClient(connectionString, topicName);
            var subscriptionClient = new SubscriptionClient(
                connectionString,
                topicName,
                this.SubscriptionName,
                mode);

            try
            {
                await this.OnMessageAsyncTestCase(
                    topicClient.InnerSender,
                    subscriptionClient.InnerSubscriptionClient.InnerReceiver,
                    maxConcurrentCalls,
                    autoComplete,
                    messageCount);
            }
            finally
            {
                await subscriptionClient.CloseAsync();
                await topicClient.CloseAsync();
            }
        }
    }
}
﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.Tests.MessageProcessorTests
{
    [TestFixture]
    public class WhenAMessageIsReceived
    {
        public class MessageType
        {
        }

        internal class TestMessageProcessor : MessageProcessor<MessageType>
        {
            internal TestMessageProcessor(IPollingMessageReceiver messageReceiver, ILog logger) : base(messageReceiver, logger)
            {
            }

            internal bool MessageProcessed;
            internal bool ThrowException;
            internal bool ExceptionHandled;

            protected override async Task ProcessMessage(MessageType messageContent)
            {
                if (ThrowException)
                {
                    throw new Exception();
                }
                MessageProcessed = true;
            }

            protected override async Task OnError(IMessage<MessageType> message)
            {
                ExceptionHandled = true;
            }
        }

        private Mock<IPollingMessageReceiver> _messageReceiver;
        private Mock<ILog> _logger;
        private TestMessageProcessor _messageProcessor;

        [SetUp]
        public void Arrange()
        {
            _messageReceiver = new Mock<IPollingMessageReceiver>();
            _logger = new Mock<ILog>();

            _messageProcessor = new TestMessageProcessor(_messageReceiver.Object, _logger.Object);
        }

        [Test]
        public void AndTheMessageIsNullThenNoProcessingIsDone()
        {
            _messageReceiver.Setup(x => x.ReceiveAsAsync<MessageType>()).ReturnsAsync(null);

            ProcessMessage(() => !_messageProcessor.MessageProcessed);
            
            Assert.IsFalse(_messageProcessor.MessageProcessed);
        }

        [Test]
        public void AndTheMessageHasNoContentThenNoProcessingIsDoneAndTheMessageIsCompleted()
        {
            var message = new Mock<IMessage<MessageType>>();
            _messageReceiver.Setup(x => x.ReceiveAsAsync<MessageType>()).ReturnsAsync(message.Object);

            ProcessMessage(() => !_messageProcessor.MessageProcessed);

            Assert.IsFalse(_messageProcessor.MessageProcessed);
            message.Verify(x => x.CompleteAsync());
        }

        [Test]
        public void ThenTheMessageIsProcessedAndCompleted()
        {
            var message = new Mock<IMessage<MessageType>>();
            message.SetupGet(x => x.Content).Returns(new MessageType());
            _messageReceiver.Setup(x => x.ReceiveAsAsync<MessageType>()).ReturnsAsync(message.Object);

            ProcessMessage(() => _messageProcessor.MessageProcessed);

            Assert.IsTrue(_messageProcessor.MessageProcessed);
            message.Verify(x => x.CompleteAsync());
        }

        [Test]
        public void AndAnErrorOccursThenTheErrorIsLoggedAndHandled()
        {
            _messageProcessor.ThrowException = true;
            var message = new Mock<IMessage<MessageType>>();
            message.SetupGet(x => x.Content).Returns(new MessageType());
            _messageReceiver.Setup(x => x.ReceiveAsAsync<MessageType>()).ReturnsAsync(message.Object);

            ProcessMessage(() => _messageProcessor.ExceptionHandled);

            _logger.Verify(x => x.Error(It.IsAny<Exception>(), $"Failed to process message {typeof(MessageType).FullName}"));
            Assert.IsTrue(_messageProcessor.ExceptionHandled);
            Assert.IsFalse(_messageProcessor.MessageProcessed);
            message.Verify(x => x.CompleteAsync(), Times.Never);
        }

        private void ProcessMessage(Func<bool> condition)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => _messageProcessor.RunAsync(cancellationTokenSource.Token));

            var timeout = DateTime.Now.AddSeconds(1);
            while (DateTime.Now <= timeout)
            {
                Thread.Sleep(10);
                if (condition())
                {
                    break;
                }
            }
        }
    }
}
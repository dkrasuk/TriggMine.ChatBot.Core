using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriggMine.ChatBot.Core.Services;
using TriggMine.ChatBot.Repository.Interfaces;
using TriggMine.ChatBot.Repository.Models;
using TriggMine.ChatBot.Shared.DTO;

namespace Triggmine.Chatbot.Core.Tests
{
    [TestFixture]
    public class MessageServiceTests
    {
        private Mock<ILogger<MessageService>> _mockLogger;
        private Mock<TriggMine.ChatBot.Repository.Interfaces.IUnitOfWorkFactory> _mockUnitOfWorkFactory;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private List<Message> _messages;


        [SetUp]
        public void Setup()
        {
            var fakeMessages = SetQuerybleMessages();

            _messages = new List<Message>();

            _mockLogger = new Mock<ILogger<MessageService>>();
            _mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWorkFactory.Setup(x => x.Create()).Returns(_mockUnitOfWork.Object);
            _mockUnitOfWork.Setup(x => x.MessageRepository.Query()).Returns(fakeMessages);

            _mockUnitOfWork.Setup(x => x.MessageRepository.AddAsync(It.IsAny<Message>()))
                .Callback((Message m) =>
                {
                    _messages.Add(m);
                });

        }

        [Test]
        public async Task GetMessage_should_be_not_null()
        {
            // Arrange
            var messageService = new MessageService(_mockLogger.Object, _mockUnitOfWorkFactory.Object);
            // Act
            var messageDTOs = await messageService.GetMessageAsync();
            // Assert
            Assert.IsNotNull(messageDTOs);
            Assert.AreEqual(messageDTOs.Count, SetQuerybleMessages().ToList().Count);
        }

        [Test]
        public void AddMessage_should_be_equals()
        {
            // Arrange
            var messageService = new MessageService(_mockLogger.Object, _mockUnitOfWorkFactory.Object);
            // Act
            var messageDTOs = messageService.CreateMessage(new MessageDTO());
            // Assert
            Assert.IsNotNull(messageDTOs);
            Assert.AreEqual(_messages.Count, 1);
        }

        private IQueryable<Message> SetQuerybleMessages()
        {
            List<Message> messages = new List<Message>()
            {
               new Message
               {
                   ChatId = 15546313123,
                   ChatTitle = "ChatTitle_1",
                   Id = 1,
                   MessageId = 1,
                   SendMessageDate =  DateTime.Now,
                   Text = "Text, test, Text, test",
                   Type = "Type1",
                   User = new User
                   {
                       DateBlockedUser = DateTime.Now,
                       DateFirstActivity = DateTime.Now.AddDays(-5),
                       FirstName = "FirstName",
                       LastName = "LastName",
                       Messages = null,
                       IsBlocked = false,
                       IsBot = false,
                       LanguageCode = "UKR",
                       Username = "Username"
                   }

               },
               new Message
               {
                   ChatId = 432442344,
                   ChatTitle = "ChatTitle_2",
                   Id = 2,
                   MessageId = 2,
                   SendMessageDate =  DateTime.Now,
                   Text = "Text, test, Text, test",
                   Type = "Type2",
                   User = new User
                   {
                       DateBlockedUser = DateTime.Now,
                       DateFirstActivity = DateTime.Now.AddDays(-10),
                       FirstName = "FirstName",
                       LastName = "LastName",
                       Messages = null,
                       IsBlocked = false,
                       IsBot = true,
                       LanguageCode = "EU",
                       Username = "Username"
                   }
               }
            };
            return messages.AsQueryable();
        }
    }
}
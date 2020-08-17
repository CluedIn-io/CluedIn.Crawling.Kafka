using System;
using System.Linq;
using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Crawling.Factories;
using CluedIn.Crawling.Helpers;
using CluedIn.Crawling.Kafka.Core.Models;
using CluedIn.Crawling.Kafka.Vocabularies;

namespace CluedIn.Crawling.Kafka.ClueProducers
{
    public class ContactProducer : BaseClueProducer<Contact>
    {
        private readonly IClueFactory _factory;

        public ContactProducer([NotNull] IClueFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factory = factory;
        }

        protected override Clue MakeClueImpl([NotNull] Contact input, Guid accountId)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var clue = _factory.Create(EntityType.Infrastructure.User, input.ContactId, accountId);

            var data = clue.Data.EntityData;

            if (!string.IsNullOrWhiteSpace(input.FullName))
            {
                data.Name = input.FullName;
            }

            var vocab = new ContactVocabulary();

            if (!data.OutgoingEdges.Any())
                _factory.CreateEntityRootReference(clue, EntityEdgeType.PartOf);

            data.Properties[vocab.AccountId] = input.AccountId.PrintIfAvailable();
            data.Properties[vocab.AccountIdName] = input.AccountIdName.PrintIfAvailable();
            data.Properties[vocab.AccountIdYomiName] = input.AccountIdYomiName.PrintIfAvailable();
            data.Properties[vocab.AccountRoleCode] = input.AccountRoleCode.PrintIfAvailable();
            data.Properties[vocab.AccountRoleCodeName] = input.AccountRoleCodeName.PrintIfAvailable();
            data.Properties[vocab.ContactId] = input.ContactId.PrintIfAvailable();
            data.Properties[vocab.Description] = input.Description.PrintIfAvailable();
            data.Properties[vocab.FullName] = input.FullName.PrintIfAvailable();
            data.Properties[vocab.NickName] = input.NickName.PrintIfAvailable();

            return clue;
        }
    }
}

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.Crawling.Kafka.Vocabularies
{
    public class KafkaVocabulary : SimpleVocabulary
    {
        public KafkaVocabulary()
        {
            VocabularyName = "Kafka Contact"; // TODO: Set value
            KeyPrefix = "kafka.contact"; // TODO: Set value
            KeySeparator = ".";
            Grouping = EntityType.Infrastructure.User; // TODO: Set value

            AddGroup("Kafka Contact Details", group =>
            {
                AccountId = group.Add(new VocabularyKey("AccountId", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                AccountIdName = group.Add(new VocabularyKey("AccountIdName", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                AccountIdYomiName = group.Add(new VocabularyKey("AccountIdYomiName", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                AccountRoleCode = group.Add(new VocabularyKey("AccountRoleCode", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                AccountRoleCodeName = group.Add(new VocabularyKey("AccountRoleCodeName", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                ContactId = group.Add(new VocabularyKey("ContactId", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                Description = group.Add(new VocabularyKey("Description", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
                FullName = group.Add(new VocabularyKey("FullName", VocabularyKeyDataType.PersonName, VocabularyKeyVisibility.Visible));
                NickName = group.Add(new VocabularyKey("NickName", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));

            });

            AddMapping(FullName, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.FullName);

        }

        public VocabularyKey AccountId { get; internal set; }
        public VocabularyKey AccountIdName { get; internal set; }
        public VocabularyKey AccountIdYomiName { get; internal set; }
        public VocabularyKey AccountRoleCode { get; internal set; }
        public VocabularyKey AccountRoleCodeName { get; internal set; }
        public VocabularyKey ContactId { get; internal set; }
        public VocabularyKey Description { get; internal set; }
        public VocabularyKey FullName { get; internal set; }
        public VocabularyKey NickName { get; internal set; }
    }
}

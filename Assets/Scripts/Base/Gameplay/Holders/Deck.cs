using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cards.Data;


namespace Cards
{
    public class Deck : MonoBehaviour
    {
        private const int MAX_CARD_INDEX = 9;

        [Header("Components")]
        [SerializeField] private Transform cardPlace;
        [SerializeField] private Transform trumpPlace;


        private DurakCard.SuitTypes trump;
        private List<CardInfo> cardsData;
        private Dictionary<CardInfo, Card> cardsObjects;

        private List<PlayerWrapper> players;
        private CardFactory cardFactory;
        private Coroutine dealtCoroutine;

        public CardInfo TopCard
        {
            get
            {
                if (CardsData.Count > 0)
                {
                    return CardsData[CardsData.Count - 1];
                }
                return null;
            }
        }
        public List<CardInfo> CardsData
        {
            get => cardsData;
        }


        public System.Action<List<CardInfo>> OnDeckGenerated { get; set; }
        public System.Action<CardInfo, PlayerWrapper> OnSendCard { get; set; }


        public void Initilize(List<PlayerWrapper> players, CardFactory cardFactory, DurakCard.SuitTypes trump)
        {
            this.players = players;
            this.cardFactory = cardFactory;
            this.trump = trump;
        }

        public List<CardInfo> GenerateDeckData()
        {
            cardsData = new List<CardInfo>();
            for(int i = 0; i < MAX_CARD_INDEX; i++)
            {
                for (int suit = 0; suit < 4; suit++)
                {
                    cardsData.Add(new CardInfo(i, suit));
                }      
            }

            for (int i = 0; i < 5; i++)
            {
                System.Random random = new System.Random(Random.Range(-1000, 1000));
                random.Shuffle(cardsData);
            }

            CardInfo trumpInfo = cardsData.Where(x => x.suit == (int)trump).ToList().GetRandom();
            int index = cardsData.IndexOf(trumpInfo);
            cardsData[index] = cardsData[0];
            cardsData[0] = trumpInfo;

            OnDeckGenerated?.Invoke(cardsData);
            return cardsData;
        }
        public void SetDeckData(List<CardInfo> info)
        {
            cardsData = new List<CardInfo>();
            cardsData.AddRange(info);

            BuildDeck();
        }
        public void BuildDeck()
        {
            cardsObjects = new Dictionary<CardInfo, Card>();

            Vector3 offset = Vector3.zero;
            foreach(CardInfo info in cardsData)
            {
                Card card = cardFactory.CreateCard(info.index, (DurakCard.SuitTypes)info.suit);
                card.Takable = false;
                card.transform.SetParent(cardPlace);
                card.transform.localPosition = offset;
                card.transform.localRotation = Quaternion.Euler(0, 0, 180);

                cardsObjects.Add(info, card);
                offset += Vector3.up * 0.1f;
            }


            Card trump = cardsObjects[cardsData[0]];
            trump.transform.SetParent(trumpPlace);
            trump.transform.localPosition = Vector3.zero;
            trump.transform.localRotation = Quaternion.identity;
        }

        public void DealtCards()
        {
            if(dealtCoroutine != null)
            {
                StopCoroutine(dealtCoroutine);
            }
            dealtCoroutine = StartCoroutine(DealtCardsCour());
        }
        public IEnumerator DealtCardsCour()
        {
            yield return new WaitForSeconds(0.25f);
            int targetCount = 6;

            bool done = false;
            while(cardsData.Count > 0 && !done)
            {
                foreach (PlayerWrapper player in players)
                {
                    if(player.Hands.CardsCount < targetCount)
                    {
                        OnSendCard?.Invoke(TopCard, player);

                        SendCard(TopCard, player);
                    }

                    yield return new WaitForSeconds(0.15f);
                }

                done = players.Count(x => x.Hands.CardsCount < targetCount) == 0;
            }

            yield break;
        }

        public void SendCard(CardInfo info, PlayerWrapper player)
        {
            if (cardsObjects.ContainsKey(info))
            {
                player.Hands.Drop(cardsObjects[info], new DropCardData{ Sender = DropCardData.SenderTypes.Dect });

                cardsData.Remove(info);
                cardsObjects.Remove(info);
            }
        }
    }
}

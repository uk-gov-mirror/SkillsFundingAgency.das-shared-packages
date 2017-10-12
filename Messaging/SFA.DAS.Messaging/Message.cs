using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging
{
    public abstract class Message<T> : IMessage<T>
    {
        protected Message(T content)
        {
            Content = content;
        }
        protected Message()
        {
            
        }

        public T Content { get; protected set; }

        public abstract Task CompleteAsync();
        public abstract Task AbortAsync();
    }
}
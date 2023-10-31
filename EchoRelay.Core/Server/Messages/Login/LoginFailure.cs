using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;
using System.Net;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from server to client indicating their <see cref="LoginRequest"/> failed.
    /// </summary>
    public class LoginFailure : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -6504933290668142767;

        /// <summary>
        /// The identifier of the associated user.
        /// </summary>
        public XPlatformId UserId;

        public ulong _statusCode;
        /// <summary>
        /// The status code returned with the failure.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return (HttpStatusCode)_statusCode; }
            set { _statusCode = (ulong)value; }
        }

        /// <summary>
        /// The message returned with the failure.
        /// </summary>
        public string Message;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LoginFailure"/> message.
        /// </summary>
        public LoginFailure()
        {
            UserId = new XPlatformId();
            StatusCode = HttpStatusCode.InternalServerError;
            Message = "";
        }
        /// <summary>
        /// Initializes a new <see cref="LoginFailure"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The identifier of the associated user.</param>
        /// <param name="statusCode">The status code returned with the failure.</param>
        /// <param name="message">The message returned with the failure.</param>
        public LoginFailure(XPlatformId userId, HttpStatusCode statusCode, string message)
        {
            UserId = userId;
            StatusCode = statusCode;
            Message = message;
        }


        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            UserId.Stream(io);
            io.Stream(ref _statusCode);
            io.Stream(ref Message, true);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(user_id={UserId}, status={StatusCode}, msg=\"{Message}\")";
        }
        #endregion
    }
}

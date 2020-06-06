using System;
using SharpBrick.PoweredUp.Protocol.Messages;

namespace SharpBrick.PoweredUp.Protocol.Formatter
{
    public class PortOutputCommandEncoder : IMessageContentEncoder
    {
        public ushort CalculateContentLength(PoweredUpMessage message)
            => (ushort)(message switch
            {
                PortOutputCommandMessage portOutputMessage => 3 + portOutputMessage switch
                {
                    PortOutputCommandWriteDirectModeDataMessage directWriteModeDataMessage => 1 + directWriteModeDataMessage switch
                    {
                        PortOutputCommandSetRgbColorNoMessage msg => 1,
                        PortOutputCommandSetRgbColorNo2Message msg => 3,

                        _ => throw new NotSupportedException(),
                    },

                    _ => throw new NotSupportedException(),
                },

                _ => throw new NotSupportedException(),
            });

        public PoweredUpMessage Decode(in Span<byte> data)
            => throw new NotImplementedException();

        public void Encode(PoweredUpMessage message, in Span<byte> data)
            => Encode(message as PortOutputCommandMessage ?? throw new InvalidOperationException(), data);

        public void Encode(PortOutputCommandMessage message, in Span<byte> data)
        {
            data[0] = message.PortId;
            data[1] = (byte)(((byte)message.StartupInformation) << 4 | ((byte)message.CompletionInformation));
            data[2] = (byte)message.SubCommand;

            switch (message)
            {
                case PortOutputCommandWriteDirectModeDataMessage directWriteModeDataMessage:
                    data[3] = directWriteModeDataMessage.Mode;

                    switch (directWriteModeDataMessage)
                    {
                        case PortOutputCommandSetRgbColorNoMessage msg:
                            data[4] = (byte)msg.ColorNo;
                            break;

                        case PortOutputCommandSetRgbColorNo2Message msg:
                            data[4] = msg.RedColor;
                            data[5] = msg.GreenColor;
                            data[6] = msg.BlueColor;
                            break;
                    }

                    break;
            }
        }
    }
}
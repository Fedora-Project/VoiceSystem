using BrokeProtocol.API;
using BrokeProtocol.Collections;
using BrokeProtocol.Entities;
using BrokeProtocol.Utility;
using BrokeProtocol.Utility.Networking;
using ENet;

namespace VoiceChat
{
    internal class VoiceChat : PlayerEvents
    {
        [Execution(ExecutionMode.Override)]
        public override bool ChatVoice(ShPlayer player, byte[] voiceData)
        {
            if (player.svPlayer.callTarget && player.svPlayer.callActive)
            {
                player.svPlayer.callTarget.svPlayer.Send(SvSendType.Self, Channel.Unreliable, ClPacket.ChatVoiceCall, player.ID, voiceData);
            }
            else
            {
                switch (player.chatMode)
                {
                    case ChatMode.Public:
                        foreach (var p in EntityCollections.Humans)
                        {
                            if (p != player && player.DistanceSqr(p) < 1056f)
                            {
                                p.svPlayer.Send(SvSendType.Self, Channel.Unreliable, ClPacket.ChatVoice, player.ID, voiceData);
                            }
                        }
                        break;
                    case ChatMode.Channel:
                        foreach (var p in EntityCollections.Humans)
                        {
                            if (p.chatChannel == player.chatChannel && p != player && player.HasItem(new ShPhone()) && p.chatMode == player.chatMode)
                            {
                                p.svPlayer.Send(SvSendType.Self, Channel.Unreliable, ClPacket.ChatVoiceChannel, player.ID, voiceData);
                            }
                        }
                        break;
                }
            }
            return true;
        }

        [Execution(ExecutionMode.Override)]
        public override bool SetChatMode(ShPlayer player, ChatMode chatMode)
        {
            if (chatMode != ChatMode.Job)
            {
                player.chatMode = chatMode;
                player.svPlayer.Send(SvSendType.Self, PacketFlags.Reliable, ClPacket.SetChatMode, (byte)player.chatMode);
            }
            return true;
        }
    }
}

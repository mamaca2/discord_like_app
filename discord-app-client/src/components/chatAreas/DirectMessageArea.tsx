import { useEffect, useRef, useState } from "react";
import type { HubConnection } from "@microsoft/signalr";

import {
  getConversation,
  type DirectMessageResponseDto
} from "../../services/directMessageService";

import MessageInput from "./MessageInput";
import MessageList from "./MessageList";

import styles from "../../css/chatAreaCss/ChatArea.module.css";

interface DirectMessageAreaProps {
  friendId: string;
  token: string;
  connection: HubConnection | null;
  onSendMessage: (friendId: string, content: string) => Promise<void>;
  onProfileClicked: (friendId: string) => void;
}

export default function DirectMessageArea({
  friendId,
  token,
  connection,
  onSendMessage,
  onProfileClicked
}: DirectMessageAreaProps) {
  const [messages, setMessages] = useState<DirectMessageResponseDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const messagesRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    let cancelled = false;

    setMessages([]);
    setLoading(true);
    setError(null);

    getConversation(token, friendId)
      .then(data => {
        if (cancelled) return;

        setMessages(prev => {
          const combined = new Map<number, DirectMessageResponseDto>();

          data.forEach(message => {
            combined.set(message.id, message);
          });

          prev.forEach(message => {
            if (
              message.senderId === friendId ||
              message.receiverId === friendId
            ) {
              combined.set(message.id, message);
            }
          });

          return Array.from(combined.values()).sort(
            (a, b) =>
              new Date(a.sentAt).getTime() -
              new Date(b.sentAt).getTime() ||
              a.id - b.id
          );
        });
      })
      .catch(error => {
        if (!cancelled) {
          setError("Failed to load conversation.");
          console.error(error);
        }
      })
      .finally(() => {
        if (!cancelled) {
          setLoading(false);
        }
      });

    return () => {
      cancelled = true;
    };
  }, [friendId, token]);

  useEffect(() => {
    if (!connection) return;

    const receiveMessage = (message: DirectMessageResponseDto) => {
      if (
        message.senderId !== friendId &&
        message.receiverId !== friendId
      ) {
        return;
      }

      setMessages(prev => {
        if (prev.some(m => m.id === message.id)) {
          return prev;
        }

        return [...prev, message].sort(
          (a, b) =>
            new Date(a.sentAt).getTime() -
            new Date(b.sentAt).getTime() ||
            a.id - b.id
        );
      });
    };

    connection.on("ReceiveMessage", receiveMessage);

    return () => {
      connection.off("ReceiveMessage", receiveMessage);
    };
  }, [connection, friendId]);

  useEffect(() => {
    const container = messagesRef.current;

    if (container) {
      container.scrollTop = container.scrollHeight;
    }
  }, [messages, loading]);

  const handleSendMessage = async (content: string) => {
    await onSendMessage(friendId, content);
  };

  return (
    <div className={styles.chatArea}>
      <header className={styles.header}>
        <button
          type="button"
          className={styles.profileButton}
          onClick={() => onProfileClicked(friendId)}
        >
          <span className={styles.avatar}>F</span>

          <div className={styles.headerInfo}>
            <span className={styles.headerTitle}>Friend</span>
            <span className={styles.headerSubtitle}>Direct message</span>
          </div>
        </button>
      </header>

      <div className={styles.messages} ref={messagesRef}>
        {loading ? (
          <div className={styles.stateText}>
            Loading conversation...
          </div>
        ) : error ? (
          <div className={styles.stateText}>
            {error}
          </div>
        ) : messages.length === 0 ? (
          <div className={styles.emptyState}>
            <h3>Start a conversation</h3>
            <p>Send your first message below.</p>
          </div>
        ) : (
          <MessageList
            showAvatar
            messages={messages.map(message => ({
              id: message.id,
              content: message.content,
              isMine: message.senderId !== friendId,
              sentAt: message.sentAt,
              inviteCode: message.inviteCode
            }))}
          />
        )}
      </div>

      <MessageInput
        key={friendId}
        placeholder="Aa"
        onSendMessage={handleSendMessage}
        disabled={!connection}
      />
    </div>
  );
}

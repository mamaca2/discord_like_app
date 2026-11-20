import { useEffect, useRef, useState, type FormEvent } from "react";
import type { HubConnection } from "@microsoft/signalr";

import {
  getConversation,
  type DirectMessageResponseDto
} from "../../services/directMessageService";

import styles from "../css/ChatArea.module.css";

interface ChatAreaProps {
  friendId: string;
  token: string;
  connection: HubConnection | null;
  onSendMessage: (friendId: string, content: string) => Promise<void>;
  onProfileClicked: (friendId: string) => void;
}

export default function ChatArea({
  friendId,
  token,
  connection,
  onSendMessage,
  onProfileClicked
}: ChatAreaProps) {
  const [messages, setMessages] = useState<DirectMessageResponseDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [messageInput, setMessageInput] = useState("");
  const [sending, setSending] = useState(false);

  const messagesRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    let cancelled = false;

    setMessages([]);
    setMessageInput("");
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

  const handleSendMessage = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const content = messageInput.trim();

    if (!content || sending) return;

    setSending(true);

    try {
      await onSendMessage(friendId, content);
      setMessageInput("");
    } catch (error) {
      console.error("Failed to send message:", error);
    } finally {
      setSending(false);
    }
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
          <div className={styles.stateText}>{error}</div>
        ) : messages.length === 0 ? (
          <div className={styles.emptyState}>
            <h3>Start a conversation</h3>
            <p>Send your first message below.</p>
          </div>
        ) : (
          messages.map(message => {
            const isMine = message.senderId !== friendId;

            return (
              <div
                key={message.id}
                className={`${styles.messageRow} ${
                  isMine ? styles.mine : styles.theirs
                }`}
              >
                {!isMine && (
                  <div className={styles.messageAvatar}>F</div>
                )}

                <div className={styles.messageStack}>
                  <div className={styles.bubble}>
                    {message.content}
                  </div>

                  <span className={styles.timestamp}>
                    {new Date(message.sentAt).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit"
                    })}
                  </span>
                </div>
              </div>
            );
          })
        )}
      </div>

      <div className={styles.composerWrapper}>
        <form
          className={styles.messageForm}
          onSubmit={handleSendMessage}
        >
          <input
            className={styles.messageInput}
            type="text"
            value={messageInput}
            onChange={event => setMessageInput(event.target.value)}
            placeholder="Aa"
            aria-label="Message content"
          />

          <button
            className={styles.sendButton}
            type="submit"
            disabled={!messageInput.trim() || sending || !connection}
            aria-label="Send message"
          >
            ➤
          </button>
        </form>
      </div>
    </div>
  );
}


import { useEffect, useRef, useState, type FormEvent } from "react";
import {
  getConversation,
  type DirectMessageResponseDto
} from "../../services/directMessageService";
import styles from "../css/ChatArea.module.css";

interface ChatAreaProps {
  friendId: string;
  token: string;
  onSendMessage: (friendId: string, content: string) => Promise<void>;
  onProfileClicked: (friendId: string) => void;
}

export default function ChatArea({
  friendId,
  token,
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
        if (!cancelled) {
          setMessages(data);
        }
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
      console.error(error);
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
          <span className={styles.avatar}>DM</span>
          <span className={styles.headerTitle}>Direct Message</span>
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
          messages.map(message => (
            <div key={message.id} className={styles.message}>
              <div className={styles.messageAvatar}>
                {message.senderId === friendId ? "F" : "Y"}
              </div>

              <div className={styles.messageBody}>
                <div className={styles.messageInfo}>
                  <span className={styles.sender}>
                    {message.senderId === friendId ? "Friend" : "You"}
                  </span>
                  <time className={styles.timestamp}>
                    {new Date(message.sentAt).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit"
                    })}
                  </time>
                </div>

                <p className={styles.content}>{message.content}</p>
              </div>
            </div>
          ))
        )}
      </div>

      <div className={styles.composerWrapper}>
        <form className={styles.messageForm} onSubmit={handleSendMessage}>
          <input
            className={styles.messageInput}
            type="text"
            value={messageInput}
            onChange={event => setMessageInput(event.target.value)}
            placeholder="Message your friend..."
            aria-label="Message content"
          />

          <button
            className={styles.sendButton}
            type="submit"
            disabled={!messageInput.trim() || sending}
          >
            {sending ? "Sending..." : "Send"}
          </button>
        </form>
      </div>
    </div>
  );
}

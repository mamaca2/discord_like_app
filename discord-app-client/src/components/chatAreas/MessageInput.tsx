import { useState, type FormEvent } from "react";
import styles from "../../css/chatAreaCss/MessageInput.module.css";

interface MessageInputProps {
  placeholder: string;
  onSendMessage: (content: string) => Promise<void>;
  disabled?: boolean;
}

export default function MessageInput({
  placeholder,
  onSendMessage,
  disabled = false
}: MessageInputProps) {
  const [message, setMessage] = useState("");
  const [sending, setSending] = useState(false);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const content = message.trim();

    if (!content || sending || disabled) return;

    setSending(true);

    try {
      await onSendMessage(content);
      setMessage("");
    } catch (error) {
      console.error("Failed to send message:", error);
    } finally {
      setSending(false);
    }
  };

  return (
    <div className={styles.composerContainer}>
      <form
        className={styles.messageForm}
        onSubmit={handleSubmit}
      >
        <input
          type="text"
          className={styles.messageInput}
          placeholder={placeholder}
          value={message}
          onChange={e => setMessage(e.target.value)}
          disabled={disabled || sending}
          aria-label="Message content"
        />

        <button
          type="submit"
          className={styles.sendButton}
          disabled={!message.trim() || sending || disabled}
          aria-label="Send message"
        >
          {sending ? "Sending..." : "Send"}
        </button>
      </form>
    </div>
  );
}

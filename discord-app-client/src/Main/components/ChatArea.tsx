import styles from "./ChatArea.module.css";

interface ChatAreaProps {
  title: string | null;
}

export default function ChatArea({ title }: ChatAreaProps) {
  if (!title) return <div className={styles.chat} />;

  return (
    <div className={styles.chat}>
      <div className={styles.header}>{title}</div>
      <div className={styles.messages} />
      <div className={styles.inputBar}>
        <input className={styles.input} placeholder={`Message ${title}`} />
      </div>
    </div>
  );
}
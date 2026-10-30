import styles from "./ServerSidebar.module.css";

export interface Channel {
  id: number;
  name: string;
  type: "text" | "voice";
}

// Mock data until the backend has channels
const MOCK_CHANNELS: Channel[] = [
  { id: 1, name: "general", type: "text" },
  { id: 2, name: "random", type: "text" },
  { id: 3, name: "Lounge", type: "voice" },
];

interface ServerSidebarProps {
  serverName: string;
  activeChannelId: number | null;
  onSelectChannel: (channel: Channel) => void;
}

export default function ServerSidebar({
  serverName,
  activeChannelId,
  onSelectChannel,
}: ServerSidebarProps) {
  const render = (type: Channel["type"], prefix: string) =>
    MOCK_CHANNELS.filter((c) => c.type === type).map((c) => (
      <button
        type="button"
        key={c.id}
        className={`${styles.channelButton} ${
          activeChannelId === c.id ? styles.active : ""
        }`}
        onClick={() => onSelectChannel(c)}
      >
        {prefix} {c.name}
      </button>
    ));

  return (
    <div className={styles.sidebar}>
      <div className={styles.serverName}>{serverName}</div>

      <div className={styles.sectionTitle}>Chats</div>
      {render("text", "#")}

      <div className={styles.sectionTitle}>Calls</div>
      {render("voice", "🔊")}
    </div>
  );
}
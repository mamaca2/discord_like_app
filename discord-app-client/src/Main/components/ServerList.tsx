import type { ServerDto } from "../../services/serverService";
import styles from "../css/ServerList.module.css";

interface ServerListProps {
  servers: ServerDto[];
  activeServerId: number | null;
  activeBar: "friends" | "server" | "search";
  onSelectFriends: () => void;
  onSelectServer: (serverId: number) => void;
}

export function ServerList({
  servers,
  activeServerId,
  activeBar,
  onSelectFriends,
  onSelectServer,
}: ServerListProps) {
  return (
    <nav className={styles.serverSidebar}>
      <button
        type="button"
        className={`${styles.friendsIcon} ${activeBar !== "server" ? styles.activeTab : ""}`}
        onClick={onSelectFriends}
        title="Direct Messages & Friends"
      >
        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
          <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
        </svg>
      </button>

      <div className={styles.separator} />

      {servers.map((server) => {
        const isActive = activeBar === "server" && activeServerId === server.id;

        return (
          <button
            type="button"
            key={server.id}
            className={`${styles.serverIcon} ${isActive ? styles.activeServer : ""}`}
            onClick={() => onSelectServer(server.id)}
            title={server.name}
          >
            {server.image ? (
              <img
                src={server.image}
                alt={server.name}
                className={styles.serverImage}
              />
            ) : (
              server.name.substring(0, 2).toUpperCase()
            )}
          </button>
        );
      })}

      <button
        type="button"
        className={styles.settingsIcon}
        onClick={() => console.log("Settings clicked")}
        title="Settings"
      >
        <svg
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          strokeLinecap="round"
          strokeLinejoin="round"
        >
          <path d="M12 8a4 4 0 1 0 0 8 4 4 0 0 0 0-8Z" />
          <path d="M19.4 15a1.7 1.7 0 0 0 .34 1.88l.06.06-2.86 2.86-.06-.06A1.7 1.7 0 0 0 15 19.4a1.7 1.7 0 0 0-1.4 1.6V21h-4v-.08A1.7 1.7 0 0 0 8 19.4a1.7 1.7 0 0 0-1.88.34l-.06.06-2.86-2.86.06-.06A1.7 1.7 0 0 0 3.6 15 1.7 1.7 0 0 0 2 13.6H2v-4h.08A1.7 1.7 0 0 0 3.6 8a1.7 1.7 0 0 0-.34-1.88L3.2 6.06 6.06 3.2l.06.06A1.7 1.7 0 0 0 8 3.6 1.7 1.7 0 0 0 9.4 2H13.6A1.7 1.7 0 0 0 15 3.6a1.7 1.7 0 0 0 1.88-.34l.06-.06 2.86 2.86-.06.06A1.7 1.7 0 0 0 19.4 8 1.7 1.7 0 0 0 21 9.4V13.6A1.7 1.7 0 0 0 19.4 15Z" />
        </svg>
      </button>
    </nav>
  );
}
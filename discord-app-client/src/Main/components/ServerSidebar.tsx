import type { ServerDto } from "../../services/serverService";
import styles from "../MainPage.module.css";

interface ServerSidebarProps {
  servers: ServerDto[];
  activeServerId: number | null;
  isFriendsActive: boolean;
  onSelectFriends: () => void;
  onSelectServer: (serverId: number) => void;
}

export function ServerSidebar({
  servers,
  activeServerId,
  isFriendsActive,
  onSelectFriends,
  onSelectServer,
}: ServerSidebarProps) {
  return (
    <nav className={styles.serverSidebar}>
      {/* 1. Direct Messages / Friends Icon */}
      <div
        className={`${styles.friendsIcon} ${isFriendsActive ? styles.activeTab : ""}`}
        onClick={onSelectFriends}
        title="Direct Messages & Friends"
      >
        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
          <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
        </svg>
      </div>

      <div className={styles.separator} />

      {/* 2. Dynamically Rendered Server Icons */}
      {servers.map((server) => {
        const isActive = !isFriendsActive && activeServerId === server.id;

        return (
          <div
            key={server.id}
            className={`${styles.serverIcon} ${isActive ? styles.activeServer : ""}`}
            onClick={() => onSelectServer(server.id)}
            title={server.name}
          >
            {/* Updated server.iconUrl to server.image */}
            {server.image ? (
              <img src={server.image} alt={server.name} className={styles.serverImage} />
            ) : (
              server.name.substring(0, 2).toUpperCase()
            )}
          </div>
        );
      })}
    </nav>
  );
}
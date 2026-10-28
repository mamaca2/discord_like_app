import { useState, useEffect } from "react";
import { ServerSidebar } from "./components/ServerSidebar";
import { getUserServers, type ServerDto } from "../services/serverService";

interface MainPageProps {
  token: string;
}

export default function MainPage({ token }: MainPageProps) {
  const [isFriendsActive, setIsFriendsActive] = useState<boolean>(true);
  const [activeServerId, setActiveServerId] = useState<number | null>(null);
  const [servers, setServers] = useState<ServerDto[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    async function fetchServers() {
      try {
        setIsLoading(true);
        const userServers = await getUserServers(token);
        if (isMounted) {
          setServers(userServers);
        }
      } catch (err) {
        if (isMounted) {
          setError(err instanceof Error ? err.message : "Failed to load servers");
        }
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    }

    fetchServers();

    return () => {
      isMounted = false;
    };
  }, [token]);

  const handleSelectFriends = () => {
    setIsFriendsActive(true);
    setActiveServerId(null);
  };

  const handleSelectServer = (serverId: number) => {
    setIsFriendsActive(false);
    setActiveServerId(serverId);
  };

  return (
    <div style={{ display: "flex", height: "100vh" }}>
      <ServerSidebar
        servers={servers}
        activeServerId={activeServerId}
        isFriendsActive={isFriendsActive}
        onSelectFriends={handleSelectFriends}
        onSelectServer={handleSelectServer}
      />

      {/* Main Content Area */}
      <div style={{ flex: 1, padding: "16px" }}>
        {isLoading && <div>Loading servers...</div>}
        {error && <div style={{ color: "#f23f43" }}>{error}</div>}

        {!isLoading && !error && (
          <>
            {isFriendsActive ? (
              <div>Friends List Page Content</div>
            ) : (
              <div>Server Content for ID: {activeServerId}</div>
            )}
          </>
        )}
      </div>
    </div>
  );
}
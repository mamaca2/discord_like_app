import { useState, useEffect } from "react";

import { ServerList } from "./ServerList";
import FriendsBar from "./FriendsBar";
import DirectMessageArea from "./chatAreas/DirectMessageArea";
import { SearchBar } from "./SearchBar";
import ServerBar from "./ServerBar";

import {
  getUserServers,
  createServer,
  type ServerDto,
  type ServerChatDto,
  getServerChats,
  createServerChat,
  inviteFriendToServer
} from "../services/serverService";

import { getUserFriends, type FriendDto } from "../services/friendsService";

import {
  acceptFriendRequest,
  declineFriendRequest,
  getIncomingFriendRequests,
  getOutgoingFriendRequests,
  type FriendRequestDto
} from "../services/friendRequestService";

import styles from "../css/MainPage.module.css";
import middleStyles from "../css/MiddleBar.module.css";

import { FriendRequests } from "./FriendRequests";
import Settings from "./Settings";

import * as signalR from "@microsoft/signalr";
import ServerChatArea from "./chatAreas/ServerChatArea";

interface MainPageProps {
  token: string;
  onLogout: () => void;
}

type ActiveBar = "friends" | "server" | "search";

type ActiveContent =
  | { type: "chat"; friendId: string }
  | { type: "serverChat"; chat: ServerChatDto }
  | { type: "friendRequests" }
  | { type: "settings" }
  | { type: "empty" };

export default function MainPage({ token, onLogout }: MainPageProps) {
  const [activeServerId, setActiveServerId] = useState<number | null>(null);
  const [activeBar, setActiveBar] = useState<ActiveBar>("friends");

  const [activeContent, setActiveContent] = useState<ActiveContent>({
    type: "empty"
  });

  const activeFriendId =
    activeContent.type === "chat" ? activeContent.friendId : null;

  const [servers, setServers] = useState<ServerDto[]>([]);
  const [friends, setFriends] = useState<FriendDto[]>([]);

  const [incomingFriendRequests, setIncomingFriendRequests] =
    useState<FriendRequestDto[]>([]);

  const [outgoingFriendRequests, setOutgoingFriendRequests] =
    useState<FriendRequestDto[]>([]);

  const [chatsByServer, setChatsByServer] = useState<
    Map<number, ServerChatDto[]>
  >(new Map());

  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const [connection, setConnection] =
    useState<signalR.HubConnection | null>(null);

  useEffect(() => {
    let isMounted = true;

    async function fetchData() {
      try {
        setIsLoading(true);
        setError(null);

        const [
          userServers,
          userFriends,
          incoming,
          outgoing
        ] = await Promise.all([
          getUserServers(token),
          getUserFriends(token),
          getIncomingFriendRequests(token),
          getOutgoingFriendRequests(token)
        ]);

        if (isMounted) {
          setServers(userServers);
          setFriends(userFriends);
          setIncomingFriendRequests(incoming);
          setOutgoingFriendRequests(outgoing);
        }

        const results = await Promise.all(
          userServers.map(async server => {
            const chats = await getServerChats(token, server.id);
            return [server.id, chats] as const;
          })
        );

        if (isMounted) {
          setChatsByServer(new Map(results));
        }

      } catch (error) {
        if (isMounted) {
          setError(
            error instanceof Error ? error.message : "Failed to load data"
          );
        }
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    }

    fetchData();

    return () => {
      isMounted = false;
    };
  }, [token]);

  useEffect(() => {
    let cancelled = false;

    const hub = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/direct-messages", {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    const startPromise = hub.start()
      .then(() => {
        if (!cancelled) {
          console.log("SignalR connected");
          setConnection(hub);
        }
      })
      .catch(error => {
        if (!cancelled) {
          console.error("SignalR connection failed:", error);
        }
      });

    return () => {
      cancelled = true;
      void startPromise.then(() => hub.stop());
    };
  }, [token]);

  const handleSelectFriends = () => {
    setActiveBar("friends");
    setActiveServerId(null);
  };

  const handleSelectServer = (serverId: number) => {
    setActiveServerId(serverId);
    setActiveBar("server");
    setActiveContent({ type: "empty" });
  };

  const handleSelectFriend = (friendId: string) => {
    setActiveBar("friends");
    setActiveServerId(null);
    setActiveContent({ type: "chat", friendId });
  };

  const handleAddFriendClicked = () => {
    setActiveBar("search");
  };

  const handleShowingFriendRequests = () => {
    setActiveContent({ type: "friendRequests" });
  };

  const handleServerIconClicked = () => {
    setActiveContent({ type: "settings" });
  };

  const handleSelectChat = (chat: ServerChatDto) => {
    setActiveContent({ type: "serverChat", chat });
  };

  const appendOutgoingFriendRequests = (request: FriendRequestDto) => {
    setOutgoingFriendRequests(prev => [...prev, request]);
  };

  const handleAcceptRequest = async (requestId: number) => {
    await acceptFriendRequest(token, requestId);

    setIncomingFriendRequests(prev =>
      prev.filter(request => request.id !== requestId)
    );

    try {
      const updatedFriends = await getUserFriends(token);
      setFriends(updatedFriends);
    } catch (error) {
      console.error("Failed to refresh friends:", error);
    }
  };

  const handleDeclineRequest = async (requestId: number) => {
    await declineFriendRequest(token, requestId);

    setIncomingFriendRequests(prev =>
      prev.filter(request => request.id !== requestId)
    );
  };

  const handleCancelRequest = async (requestId: number) => {
    await declineFriendRequest(token, requestId);

    setOutgoingFriendRequests(prev =>
      prev.filter(request => request.id !== requestId)
    );
  };

  const handleAddServer = async (name: string, image: File | null) => {
    try {
      const serverDto = await createServer(token, name, image);

      setServers(prev => [...prev, serverDto]);
    } catch (error) {
      console.error("Failed to create server:", error);
    }
  };

  const handleSendMessage = async (friendId: string, content: string) => {
    if (connection?.state !== signalR.HubConnectionState.Connected) {
      throw new Error("SignalR is not connected");
    }

    await connection.invoke("SendMessage", {
      receiverId: friendId,
      content: content
    });
  };

  const handleAddChat = async (name: string) => {
    if (activeServerId === null) return;

    const serverId = activeServerId;

    const newChat = await createServerChat(token, serverId, name);

    setChatsByServer(prev => {
      const updated = new Map(prev);
      const chats = updated.get(serverId) ?? [];

      updated.set(serverId, [...chats, newChat]);

      return updated;
    });
  };
  const handleInviteFriend = async (friendId: string) => {
    if (activeServerId === null) return;

    await inviteFriendToServer(token, activeServerId, friendId);
  };

  if (isLoading) {
    return <div className={styles.container}>Loading...</div>;
  }

  if (error) {
    return (
      <div className={styles.container}>
        <p>{error}</p>
        <button onClick={onLogout}>Log out</button>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <ServerList
        servers={servers}
        activeServerId={activeServerId}
        activeBar={activeBar}
        onSelectFriends={handleSelectFriends}
        onSelectServer={handleSelectServer}
        onSettingsClick={handleServerIconClicked}
        onAddServer={handleAddServer}
      />

      <aside className={middleStyles.middleBar}>
        {activeBar === "friends" && (
          <FriendsBar
            friends={friends}
            activeFriendId={activeFriendId}
            onSelectFriend={handleSelectFriend}
            onAddFriend={handleAddFriendClicked}
            onShowRequests={handleShowingFriendRequests}
          />
        )}

        {activeBar === "search" && (
          <SearchBar
            token={token}
            friends={friends}
            incomingRequests={incomingFriendRequests}
            outgoingRequests={outgoingFriendRequests}
            appendOutgoingFriendRequests={appendOutgoingFriendRequests}
            onAcceptRequest={handleAcceptRequest}
            onDeclineRequest={handleDeclineRequest}
            onCancelRequest={handleCancelRequest}
          />
        )}

        {activeBar === "server" && activeServerId !== null && (
          <ServerBar
            key={activeServerId}
            chats={chatsByServer.get(activeServerId) ?? []}
            friends={friends}
            onAddChat={handleAddChat}
            onSelectChat={handleSelectChat}
            onInviteFriend={handleInviteFriend}
          />
        )}
      </aside>

      {activeContent.type === "friendRequests" && (
        <FriendRequests
          incomingRequests={incomingFriendRequests}
          outgoingRequests={outgoingFriendRequests}
          onAccept={handleAcceptRequest}
          onDecline={handleDeclineRequest}
        />
      )}

      {activeContent.type === "settings" && (
        <Settings onLogout={onLogout} />
      )}

      {activeContent.type === "chat" && (
        <DirectMessageArea
          key={activeContent.friendId}
          friendId={activeContent.friendId}
          token={token}
          connection={connection}
          onSendMessage={handleSendMessage}
          onProfileClicked={handleSelectFriend}
        />
      )}

      {activeContent.type === "serverChat" && (
        <ServerChatArea
          key={activeContent.chat.id}
          chat={activeContent.chat}
          token={token}
        />
      )}
    </div>
  );
}

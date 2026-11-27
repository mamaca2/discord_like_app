import {
  useEffect,
  useLayoutEffect,
  useRef,
  useState
} from "react";

import * as signalR from "@microsoft/signalr";

import {
  type ServerChatDto,
  type ServerChatMessageDto,
  getServerChatMessages
} from "../../services/serverService";

import MessageInput from "./MessageInput";
import MessageList from "./MessageList";

import styles from "../../css/chatAreaCss/ServerChatArea.module.css";

interface ServerChatAreaProps {
  chat: ServerChatDto;
  token: string;
}

const PAGE_SIZE = 20;

export default function ServerChatArea({
  chat,
  token
}: ServerChatAreaProps) {
  const [messages, setMessages] = useState<ServerChatMessageDto[]>([]);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [retry, setRetry] = useState(0);
  const [connected, setConnected] = useState(false);

  const messagesRef = useRef<HTMLDivElement>(null);
  const fetchingRef = useRef(true);
  const hubRef = useRef<signalR.HubConnection | null>(null);

  const scrollRef = useRef<
    "bottom" | { height: number; top: number } | null
  >("bottom");

  useEffect(() => {
    let cancelled = false;

    fetchingRef.current = true;
    setLoading(true);
    setError(null);

    async function fetchMessages() {
      try {
        const newMessages = await getServerChatMessages(
          token,
          chat.serverId,
          chat.id,
          page,
          PAGE_SIZE
        );

        if (cancelled) return;

        setMessages(prev => {
          const combined = new Map<number, ServerChatMessageDto>();

          [...newMessages, ...prev].forEach(message => {
            combined.set(message.id, message);
          });

          return Array.from(combined.values()).sort(
            (a, b) => a.id - b.id
          );
        });

        setHasMore(newMessages.length === PAGE_SIZE);
      } catch (error) {
        if (!cancelled) {
          setError(
            error instanceof Error
              ? error.message
              : "Failed to load messages"
          );
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
          fetchingRef.current = false;
        }
      }
    }

    fetchMessages();

    return () => {
      cancelled = true;
    };
  }, [chat.id, chat.serverId, token, page, retry]);

  useEffect(() => {
    let cancelled = false;

    const hub = new signalR.HubConnectionBuilder()
      .withUrl("/hubs/server-chats", {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    const receiveMessage = (message: ServerChatMessageDto) => {
      if (message.chatId !== chat.id) return;

      const container = messagesRef.current;

      if (
        message.isMine ||
        (container &&
          container.scrollHeight -
            container.scrollTop -
            container.clientHeight < 80)
      ) {
        scrollRef.current = "bottom";
      }

      setMessages(prev => {
        if (prev.some(m => m.id === message.id)) {
          return prev;
        }

        return [...prev, message].sort(
          (a, b) => a.id - b.id
        );
      });
    };

    hub.on("ReceiveChatMessage", receiveMessage);

    hub.onreconnecting(() => {
      if (!cancelled) {
        setConnected(false);
      }
    });

    hub.onreconnected(async () => {
      if (cancelled) return;

      try {
        await hub.invoke("JoinChat", chat.serverId, chat.id);

        if (!cancelled) {
          setConnected(true);
        }
      } catch (error) {
        console.error("Failed to rejoin chat:", error);
      }
    });

    const startPromise = hub.start()
      .then(async () => {
        if (cancelled) return;

        await hub.invoke("JoinChat", chat.serverId, chat.id);

        if (!cancelled) {
          hubRef.current = hub;
          setConnected(true);
        }
      })
      .catch(error => {
        if (!cancelled) {
          console.error("Server chat connection failed:", error);
        }
      });

    return () => {
      cancelled = true;
      hubRef.current = null;
      hub.off("ReceiveChatMessage", receiveMessage);

      void startPromise.then(() => hub.stop());
    };
  }, [token, chat.serverId, chat.id]);

  useLayoutEffect(() => {
    const container = messagesRef.current;
    if (!container) return;

    const previous = scrollRef.current;

    if (previous === "bottom") {
      container.scrollTop = container.scrollHeight;
    } else if (previous) {
      container.scrollTop =
        container.scrollHeight - previous.height + previous.top;
    }

    scrollRef.current = null;
  }, [messages]);

  const handleScroll = () => {
    const container = messagesRef.current;

    if (
      !container ||
      container.scrollTop > 50 ||
      fetchingRef.current ||
      loading ||
      !hasMore ||
      error
    ) {
      return;
    }

    scrollRef.current = {
      height: container.scrollHeight,
      top: container.scrollTop
    };

    fetchingRef.current = true;
    setPage(prev => prev + 1);
  };

  const handleSendMessage = async (content: string) => {
    const hub = hubRef.current;

    if (hub?.state !== signalR.HubConnectionState.Connected) {
      throw new Error("SignalR is not connected");
    }

    await hub.invoke(
      "SendChatMessage",
      chat.serverId,
      chat.id,
      content
    );
  };

  return (
    <div className={styles.chatArea}>
      <div className={styles.chatHeader}>
        <span className={styles.hash}>#</span>
        <h3>{chat.name}</h3>
      </div>

      <div
        className={styles.messages}
        ref={messagesRef}
        onScroll={handleScroll}
      >
        {loading && (
          <p className={styles.loading}>
            Loading messages...
          </p>
        )}

        {error && (
          <div className={styles.error}>
            <p>{error}</p>

            <button
              type="button"
              onClick={() => setRetry(prev => prev + 1)}
            >
              Retry
            </button>
          </div>
        )}

        {!loading && !error && messages.length === 0 && (
          <div className={styles.emptyMessages}>
            <h2>Welcome to #{chat.name}</h2>
            <p>This is the beginning of this chat.</p>
          </div>
        )}

        {messages.length > 0 && (
          <MessageList
            messages={messages.map(message => ({
              id: message.id,
              content: message.content,
              isMine: message.isMine
            }))}
          />
        )}
      </div>

      <MessageInput
        key={chat.id}
        placeholder={`Message #${chat.name}`}
        onSendMessage={handleSendMessage}
        disabled={!connected}
      />
    </div>
  );
}

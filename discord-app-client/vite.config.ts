// import react from '@vitejs/plugin-react'
// import { defineConfig } from 'vite'

// // https://vite.dev/config/
// export default defineConfig({
//   plugins: [react()],
// })

import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    host: "0.0.0.0",
    port: 5173,
    allowedHosts: [
      "contributive-flockiest-henrietta.ngrok-free.dev"
    ],
    proxy: {
      "/api": {
        target: "http://localhost:5187",
        changeOrigin: true,
      },
      "/hubs": {
        target: "http://localhost:5187",
        changeOrigin: true,
        ws: true,
      },
    },
  },
});
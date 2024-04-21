// const { defineConfig } = require("@vue/cli-service");
// module.exports = defineConfig({
//   transpileDependencies: true,
//   devServer: {
//     host: "localhost",
//     port: 3000,
//   },
// });

import { defineConfig } from "@vue/cli-service";
import path from "path";

export default defineConfig({
  transpileDependencies: ["vue"],
  configureWebpack: {
    resolve: {
      extensions: [".js", ".ts", ".vue", ".json"],
      alias: {
        "@": path.resolve(__dirname, "src"),
      },
    },
  },
  chainWebpack: (config) => {
    config.module
      .rule("ts")
      .test(/\.tsx?$/)
      .use("ts-loader")
      .loader("ts-loader")
      .options({
        appendTsSuffixTo: [/\.vue$/],
      });
  },
  devServer: {
    host: "localhost",
    port: 3000,
  },
});

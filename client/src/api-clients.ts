import {LibraryClient} from "./generated-client.ts";

const isProduction = import.meta.env.PROD;

const prod = "https://projectsolutionserver.fly.dev";
const dev = "http://localhost:5284";

export const finalUrl = isProduction ? prod : dev;

export const libraryApi = new LibraryClient(finalUrl)
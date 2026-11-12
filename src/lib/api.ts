import type { DailyContent } from "./types";

const configuredPrayerApiUrl = process.env.NEXT_PUBLIC_PRAYER_API_URL?.trim();
const PRAYER_API_URL =
  configuredPrayerApiUrl === undefined
    ? process.env.NODE_ENV === "development"
      ? "http://localhost:8080/api"
      : ""
    : configuredPrayerApiUrl.replace(/\/$/, "");

export function isPrayerApiConfigured(): boolean {
  return Boolean(PRAYER_API_URL);
}

export async function fetchDaily(): Promise<DailyContent> {
  if (!PRAYER_API_URL) {
    throw new Error("The daily reflection service is not configured.");
  }

  const response = await fetch(`${PRAYER_API_URL}/daily/`, { cache: "no-store" });
  if (!response.ok) {
    throw new Error(`Daily reflection request failed (${response.status}).`);
  }
  return response.json() as Promise<DailyContent>;
}

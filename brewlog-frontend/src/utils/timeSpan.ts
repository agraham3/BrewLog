// Utility functions for working with TimeSpan format (HH:MM:SS)

/**
 * Converts seconds to TimeSpan format (HH:MM:SS)
 */
export const secondsToTimeSpan = (seconds: number): string => {
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const remainingSeconds = seconds % 60;
  
  return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`;
};

/**
 * Converts TimeSpan format (HH:MM:SS) to seconds
 */
export const timeSpanToSeconds = (timeSpan: string): number => {
  const parts = timeSpan.split(':').map(Number);
  if (parts.length !== 3) {
    throw new Error('Invalid TimeSpan format. Expected HH:MM:SS');
  }
  
  const [hours, minutes, seconds] = parts;
  return hours * 3600 + minutes * 60 + seconds;
};

/**
 * Formats TimeSpan for display (removes leading zeros from hours if 0)
 */
export const formatTimeSpan = (timeSpan: string): string => {
  const parts = timeSpan.split(':');
  if (parts.length !== 3) return timeSpan;
  
  const [hours, minutes, seconds] = parts;
  
  // If hours is 00, show only MM:SS
  if (hours === '00') {
    return `${minutes}:${seconds}`;
  }
  
  return timeSpan;
};

/**
 * Creates a TimeSpan string from minutes and seconds
 */
export const createTimeSpan = (minutes: number, seconds: number = 0): string => {
  const totalSeconds = minutes * 60 + seconds;
  return secondsToTimeSpan(totalSeconds);
};
export interface Device {
  id: number;
  name: string;
  manufacturer: string;
  type: string;
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ramAmount: string;
  description: string | null;
  assignedUserId: number | null;
  assignedUserName: string | null;
}

export interface CreateDevice {
  name: string;
  manufacturer: string;
  type: string;
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ramAmount: string;
  description: string | null;
}

export interface UpdateDevice {
  name: string;
  manufacturer: string;
  type: string;
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ramAmount: string;
  description: string | null;
}
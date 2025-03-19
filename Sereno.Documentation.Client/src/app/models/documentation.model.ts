export interface Documentation {
  id: string;
  title: string;
  htmlContent: string | null;
  createdBy: string;
  createdAt: string | Date;
  topic: string;
  libraryPath: string;
  documentKey: string;
  content: string | null;
  author: string | null;
  nextCheck: Date | null;
  updatedAt: Date | null;
  updatedBy: string | null;
} 

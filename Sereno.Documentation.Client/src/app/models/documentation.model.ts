export interface Documentation {
  id: string;
  libraryPath: string;
  documentKey: string;
  title: string | null;
  content: string | null;
  htmlContent: string | null;
  author: string | null;
  nextCheck: Date | null;
  createdAt: Date | null;
  createdBy: string | null;
  updatedAt: Date | null;
  updatedBy: string | null;
} 
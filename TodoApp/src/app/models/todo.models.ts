// TypeScript interfaces matching our C# DTOs

export interface Todo {
    id: number;
    title: string;
    description?: string;
    isCompleted: boolean;
    createdAt: Date;
    updatedAt: Date;
    dueDate?: Date;
    priority: number;
    isOverdue: boolean;
    priorityText: string;
  }
  
  export interface CreateTodoDto {
    title: string;
    description?: string;
    dueDate?: Date;
    priority: number;
  }
  
  export interface UpdateTodoDto {
    title: string;
    description?: string;
    isCompleted: boolean;
    dueDate?: Date;
    priority: number;
  }
  
  export interface TodoFilterDto {
    isCompleted?: boolean;
    priority?: number;
    dueDateFrom?: Date;
    dueDateTo?: Date;
    searchTerm?: string;
    page: number;
    pageSize: number;
  }
  
  export interface TodoStats {
    total: number;
    completed: number;
    pending: number;
    overdue: number;
    byPriority: {
      low: number;
      medium: number;
      high: number;
    };
  }
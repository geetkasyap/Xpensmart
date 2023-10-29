export interface ConsizedExpense {
    expenseID: number;
    userID: number;
    amount: number;
    date: string;
    categoryID: number;
    receiptUrl: string;
    description: string;
    expenseType: string;
    isActive: boolean;
    isRecurring: boolean;
    createdOn: string;
    modifiedOn: string;
    createdBy: string;
    modifiedBy: string;
  }
  
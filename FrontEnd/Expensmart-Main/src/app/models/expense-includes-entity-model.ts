export interface IncludeConsizedExpense {
    expenseID: number;
    userID: number;
    amount: number;
    date: Date;
    categoryID: number;
    receiptUrl?: string;
    description?: string;
    expenseType?: string;
    isActive: boolean;
    isRecurring: boolean;
    createdOn: Date;
    modifiedOn: Date;
    createdBy: string;
    modifiedBy: string;
    usersEntity?: any; 
    categoriesEntity?: { 
        categoryID: number;
        name?: string;
        isActive: boolean;
        createdOn: Date;
        modifiedOn: Date;
    };
    transactionsEntities?: any[];
}
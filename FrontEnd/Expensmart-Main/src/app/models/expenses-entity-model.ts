export interface ExpensesEntity {
    expenseID:number;
    userID:number;
    amount:number;
    date:Date;
    categoryID:number;
    receiptUrl:string;
    description:string;
    expenseType:string;
    isActive:boolean;
    isRecurring:boolean;
    createdOn:Date;
    modifiedOn:Date;
    createdBy:string;
    modifiedBy:string;
    usersEntity:any;
    categoriesEntity:any;
}
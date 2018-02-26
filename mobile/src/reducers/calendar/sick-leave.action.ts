export interface ClaimSickLeave {
    type: 'CLAIM-SICK-LEAVE';
}

export const claimSickLeave = (): ClaimSickLeave => ({ type: 'CLAIM-SICK-LEAVE' });

export interface ConfirmClaimSickLeave {
    type: 'CONFIRM-CLAIM-SICK-LEAVE';
}

export const confirmSickLeave = (): ConfirmClaimSickLeave => ({ type: 'CONFIRM-CLAIM-SICK-LEAVE' });

export interface EditSickLeave {
    type: 'EDIT-SICK-LEAVE';
}

export const editSickLeave = (): EditSickLeave => ({ type: 'EDIT-SICK-LEAVE' });

export interface CompleteSickLeave {
    type: 'COMPLETE-SICK-LEAVE';
}

export const completeSickLeave = (): CompleteSickLeave => ({ type: 'COMPLETE-SICK-LEAVE' });

export interface ProlongueSickLeave {
    type: 'PROLONGUE-SICK-LEAVE';
}

export const prolongueSickLeave = (): ProlongueSickLeave => ({ type: 'PROLONGUE-SICK-LEAVE' });

export interface ConfirmProlongueSickLeave {
    type: 'CONFIRM-PROLONGUE-SICK-LEAVE';
}

export const confirmProlongueSickLeave = (): ConfirmProlongueSickLeave => ({ type: 'CONFIRM-PROLONGUE-SICK-LEAVE' });

export type SickLeaveActions = ClaimSickLeave | ConfirmClaimSickLeave | EditSickLeave | CompleteSickLeave | ProlongueSickLeave | ConfirmProlongueSickLeave;
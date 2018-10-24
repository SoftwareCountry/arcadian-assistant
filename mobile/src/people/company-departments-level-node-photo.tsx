import React, { Component } from 'react';
import { Avatar } from './avatar';

interface CompanyDepartmentsLevelNodePhotoProps {
    photoUrl: string;
    showStaffIcon?: boolean;
}

export class CompanyDepartmentsLevelNodePhoto extends Component<CompanyDepartmentsLevelNodePhotoProps> {
    public render() {
        return <Avatar photoUrl={this.props.photoUrl} useDefaultForEmployeesList={!!this.props.showStaffIcon} />;
    }
}
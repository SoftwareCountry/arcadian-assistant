import React, { Component } from 'react';
import { Photo } from '../reducers/organization/employee.model';
import { Map, is } from 'immutable';
import { Avatar } from './avatar';

interface CompanyDepartmentsLevelNodePhotoProps {
    photo: Map<string, Photo>;
    showStaffIcon?: boolean;
}

export class CompanyDepartmentsLevelNodePhoto extends Component<CompanyDepartmentsLevelNodePhotoProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodePhotoProps) {
        return !is(this.props.photo, nextProps.photo) || this.props.showStaffIcon !== nextProps.showStaffIcon;
    }

    public render() {
        const photo = this.props.photo 
            ? this.props.photo.toJS() 
            : null;

        return <Avatar photo={photo} useDefaultForEmployeesList={!!this.props.showStaffIcon} />;
    }
}
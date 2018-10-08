import React, { Component } from 'react';
import { Photo } from '../reducers/organization/employee.model';
import { Map } from 'immutable';
import { Avatar } from './avatar';

interface CompanyDepartmentsLevelNodePhotoProps {
    photo: Map<string, Photo>;
}

export class CompanyDepartmentsLevelNodePhoto extends Component<CompanyDepartmentsLevelNodePhotoProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodePhotoProps) {
        return !this.props.photo.equals(nextProps.photo);
    }

    public render() {
        const photo = this.props.photo.toJS();

        return <Avatar photo={photo} />;
    }
}
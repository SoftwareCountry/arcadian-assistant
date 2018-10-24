import React, { Component } from 'react';
import { Map, is } from 'immutable';
import { Avatar } from './avatar';
import { MapPhoto } from '../reducers/people/people.model';

interface CompanyDepartmentsLevelNodePhotoProps {
    photoUrl: string;
}

export class CompanyDepartmentsLevelNodePhoto extends Component<CompanyDepartmentsLevelNodePhotoProps> {
    public render() {
        const photo = this.props.photoUrl
            ? this.props.photoUrl
            : null;

        return <Avatar photoUrl={photo} />;
    }
}
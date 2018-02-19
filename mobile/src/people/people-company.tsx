import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { PeopleActions, navigatePeopleCompany } from '../reducers/people/people.action';

interface PeopleCompanyProps {
    routeName: string;
}

interface PeopleCompanyDispatchProps {
    navigatePeopleCompany: () => void;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
    routeName: 'Company'
});

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    navigatePeopleCompany: () => { dispatch(navigatePeopleCompany()); },
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps & PeopleCompanyDispatchProps> {  
    public componentDidMount() {
        this.props.navigatePeopleCompany();
    }

    public render() {
        return <EmployeesList />;
    }
}

export const PeopleCompany = connect(mapStateToProps, mapDispatchToProps)(PeopleCompanyImpl);
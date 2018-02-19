import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
// import { NavigationActions } from 'react-navigation';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { PeopleActions, navigatePeopleDepartment } from '../reducers/people/people.action';

interface PeopleDepartmentProps {
    routeName: string;
}

interface PeopleDepartmentDispatchProps {
    navigatePeopleDepartment: () => void;
}

const mapStateToProps = (state: AppState): PeopleDepartmentProps => ({
    routeName: 'Department'
});

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    navigatePeopleDepartment: () => { dispatch(navigatePeopleDepartment()); },
});

// const mapDispatchToProps = (dispatch: Dispatch<any>): PeopleDepartmentDispatchProps => {
//     const peopleDepartmentDidMount = { type: 'PEOPLE_DEPARTMENT' };
//     return {
//       onDidMount: () => {
//         dispatch(peopleDepartmentDidMount);
//       },
//     };
//   };

export class PeopleDepartmentImpl extends React.Component<PeopleDepartmentProps & PeopleDepartmentDispatchProps> {  
    public componentDidMount() {
        this.props.navigatePeopleDepartment();
    }

    public render() {
        return <EmployeesList />;
    }
}

export const PeopleDepartment = connect(mapStateToProps, mapDispatchToProps)(PeopleDepartmentImpl);
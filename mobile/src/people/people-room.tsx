import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { PeopleActions, navigatePeopleRoom } from '../reducers/people/people.action';

interface PeopleRoomProps {
    routeName: string;
}

interface PeopleRoomDispatchProps {
    navigatePeopleRoom: () => void;
}

const mapStateToProps = (state: AppState): PeopleRoomProps => ({
    routeName: 'Room'
});

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    navigatePeopleRoom: () => { dispatch(navigatePeopleRoom()); },
});

export class PeopleRoomImpl extends React.Component<PeopleRoomProps & PeopleRoomDispatchProps> {  
    public componentDidMount() {
        this.props.navigatePeopleRoom();
    }

    public render() {
        return <EmployeesList />;
    }
}

export const PeopleRoom = connect(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);
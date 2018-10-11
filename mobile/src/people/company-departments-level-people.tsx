import React, { Component } from 'react';
import { View, FlatList, ListRenderItemInfo } from 'react-native';
import { EmployeeIdToNode, MapEmployeeNode } from '../reducers/people/people.model';
import { Avatar } from './avatar';
import { CompanyDepartmentsLevelNodePhoto } from './company-departments-level-node-photo';
import { Photo } from '../reducers/organization/employee.model';
import { Map } from 'immutable';
import { StyledText } from '../override/styled-text';

interface CompanyDepartmentsLevelPeopleProps {
    employeeIdToNode: EmployeeIdToNode;
}

export class CompanyDepartmentsLevelPeople extends Component<CompanyDepartmentsLevelPeopleProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelPeopleProps) {
        return !this.props.employeeIdToNode.equals(nextProps.employeeIdToNode);
    }

    public render() {
        const employees: MapEmployeeNode[] = this.props.employeeIdToNode.toArray();

        return (
            <FlatList
                style={{height: '100%'}}
                data={employees}
                keyExtractor={this.keyExtractor}
                renderItem={this.renderItem}
                scrollEnabled={true}
                refreshing={false}
            />
        );
    }

    private keyExtractor = (item: MapEmployeeNode): string => item.get('employeeId') as string;

    private renderItem = ({ item }: { item: MapEmployeeNode }) => {
        const photo = item.get('photo') as Map<string, Photo>;

        return (
            //<TouchableOpacity onPress={() => this.onItemClicked(itemInfo.index)}>
                <View style={{flexDirection: 'row', flex: 1}}>
                    <View style={{height: 50, width: 20}}>
                        <CompanyDepartmentsLevelNodePhoto photo={photo} />
                    </View>
                    <View>
                        <StyledText>{item.get('name')}, </StyledText>
                        <StyledText>{item.get('position')}</StyledText>
                    </View>
                </View>
            //</TouchableOpacity>
        );
    }

}
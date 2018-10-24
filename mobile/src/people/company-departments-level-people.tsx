import React, { Component } from 'react';
import { View, FlatList } from 'react-native';
import { EmployeeIdToNode, MapEmployeeNode } from '../reducers/people/people.model';
import { StyledText } from '../override/styled-text';
import { companyDepartmentLevelPeople } from './styles';
import { CompanyDepartmentsLevelPeopleTouchable } from './company-departments-level-people-touchable';
import { employeesAZComparer } from './employee-comparer';
import { Avatar } from './avatar';

interface CompanyDepartmentsLevelPeopleProps {
    employeeIdToNode: EmployeeIdToNode;
    onPressEmployee: (employeeId: string) => void;
}

export class CompanyDepartmentsLevelPeople extends Component<CompanyDepartmentsLevelPeopleProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelPeopleProps) {
        return !this.props.employeeIdToNode.equals(nextProps.employeeIdToNode) 
            || this.props.onPressEmployee !== nextProps.onPressEmployee;
    }

    public render() {
        const employees: MapEmployeeNode[] = this.props.employeeIdToNode
            .toArray()
            .sort((a, b) => employeesAZComparer(
                { name: a.get('name') as string }, 
                { name: b.get('name') as string }
            ));

        return (
            <FlatList
                style={companyDepartmentLevelPeople.list}
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
        const photo = item.get('photoUrl');

        return (
            <CompanyDepartmentsLevelPeopleTouchable onPress={this.props.onPressEmployee} employeeId={item.get('employeeId') as string}>
                <View style={companyDepartmentLevelPeople.listItemAvator}>
                    <Avatar photoUrl={photo} />;
                </View>
                <View style={companyDepartmentLevelPeople.listItemContent}>
                    <StyledText style={companyDepartmentLevelPeople.listItemName}>
                        {`${item.get('name')}, `}
                    </StyledText>
                    <StyledText style={companyDepartmentLevelPeople.listItemPosition}>
                        {item.get('position')}
                    </StyledText>
                </View>
            </CompanyDepartmentsLevelPeopleTouchable>
        );
    }
}